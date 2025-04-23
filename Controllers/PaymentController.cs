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

            // Calculate the number of rental days
            int numberOfDays = (request.EndDate - request.StartDate).Days;
            if (numberOfDays <= 0)
            {
                return BadRequest("End date must be after start date.");
            }

            // Define the driver's fee per day
            float driverFeePerDay = 30.0f;

            // Calculate the total price
            float basePrice = car.RentPrice * numberOfDays;
            float driverFee = (request.NeedDriver && request.DriverId.HasValue) ? driverFeePerDay * numberOfDays : 0;
            float totalPrice = basePrice + driverFee;

            // Create and save the reservation
            var reservation = new Reservation
            {
                UsersId = userId,
                CarId = request.CarId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ReservedDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = totalPrice,
                DriverId = request.NeedDriver ? request.DriverId : null
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            // Create and save the payment
            var payment = new Payment
            {
                ReservationId = reservation.ReservationId,
                Amount = totalPrice,
                PaymentMethod = "Stripe",
                PaymentDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Create the Stripe checkout session
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
                CancelUrl = $"http://localhost:5124/Payment/Cancel?reservationId={reservation.ReservationId}"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { url = session.Url });
        }

        public IActionResult Success(int reservationId)
        {
            // Fetch the reservation along with the associated car and payment details
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

            if (reservation.Car != null)
            {
                reservation.Car.Status = "Unavailable";
            }
            else
            {
                Console.WriteLine($"Warning: No Car record found for ReservationId {reservationId}");
            }

            _context.SaveChanges();

            return View("Success", reservation);
        }


        public IActionResult Cancel(int reservationId)
        {
            var reservation = _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.Payment)
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            if (reservation.Status == "Confirmed")
            {
                return BadRequest("Cannot cancel a confirmed reservation.");
            }

            // Update status to cancelled
            reservation.Status = "Cancelled";
            if (reservation.Payment != null)
            {
                reservation.Payment.Status = "Refund Pending"; // Or adjust based on policy
            }

            _context.SaveChanges();

            return View("Cancellation", reservation);
        }

    }
}