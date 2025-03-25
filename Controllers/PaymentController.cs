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

            // Calculate total price based on car rent price and number of days
            int numberOfDays = (request.EndDate - request.StartDate).Days;
            float totalPrice = car.RentPrice * numberOfDays;

            // Create and save Reservation first
            var reservation = new Reservation
            {
                UsersId = userId,
                CarId = request.CarId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ReservedDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = totalPrice,
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();  // Save to get ReservationId

            // Now create Payment after ensuring ReservationId exists
            var payment = new Payment
            {
                ReservationId = reservation.ReservationId, // Ensure this links correctly
                Amount = car.RentPrice * (request.EndDate - request.StartDate).Days,
                PaymentMethod = "Stripe",
                PaymentDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Payments.Add(payment);
            _context.SaveChanges(); // Save Payment

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "php",
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
                SuccessUrl = $"http://localhost:5124/Payment/Success?reservationId={reservation.ReservationId}",
                CancelUrl = "http://localhost:5124/Reservation/List"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { url = session.Url });
        }

        public IActionResult Success(int reservationId)
        {
            var reservation = _context.Reservations
                .Include(r => r.Car)       // Load Car details
                .Include(r => r.Payment)   // Load Payment details
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            reservation.Status = "Confirmed";
            if (reservation.Payment != null)
            {
                reservation.Payment.Status = "Paid";
            }
            else
            {
                Console.WriteLine($"Warning: No Payment record found for ReservationId {reservationId}");
            }

            _context.SaveChanges();

            // Pass reservation details to the Success view
            return View("Success", reservation);
        }
    }
}