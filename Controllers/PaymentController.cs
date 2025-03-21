using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Services;
using CarRental.Models;
using Stripe.Checkout;
using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

namespace CarRental.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession([FromBody] PaymentRequest request)
        {
            var car = _context.Cars.FirstOrDefault(c => c.CarId == request.CarId);
            if (car == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var reservation = new Reservation
            {
                UsersId = userId,
                CarId = request.CarId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ReservedDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            var payment = new Payment
            {
                ReservationId = reservation.ReservationId,
                Amount = car.RentPrice * (request.EndDate - request.StartDate).Days,
                PaymentMethod = "Stripe",
                PaymentDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long)(payment.Amount * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"{car.Brand} {car.Model}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = $"http://localhost:5000/Payment/Success?reservationId={reservation.ReservationId}",
                CancelUrl = "http://localhost:5000/Reservation/List"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { url = session.Url });
        }

        public IActionResult Success(int reservationId)
        {
            var reservation = _context.Reservations.Include(r => r.Payment).FirstOrDefault(r => r.ReservationId == reservationId);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Confirmed";
            reservation.Payment.Status = "Paid";

            _context.SaveChanges();

            return RedirectToAction("UserReservations", "Reservation");
        }
    }
}