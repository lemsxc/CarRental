using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Admin/GetRevenue")]
        public async Task<IActionResult> GetRevenue(DateTime startDate, DateTime endDate)
        {
            var payments = _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .ToList();

            var totalRevenue = payments.Sum(p => p.Amount);
            var totalBookings = _context.Reservations.Count(r => r.Payment.PaymentDate >= startDate && r.Payment.PaymentDate <= endDate);
            var activeRentals = _context.Reservations.Count(r => r.Status == "Active");

            var revenueTrends = payments.GroupBy(p => p.PaymentDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(p => p.Amount) })
                .OrderBy(x => x.Date)
                .ToList();

            return Json(new
            {
                totalRevenue,
                totalBookings,
                activeRentals,
                revenueTrends = new
                {
                    labels = revenueTrends.Select(x => x.Date.ToString("yyyy-MM-dd")).ToArray(),
                    values = revenueTrends.Select(x => x.Total).ToArray()
                }
            }
            );
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Vehicles()
        {
            var cars = _context.Cars.ToList();
            return View(cars);
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Rentals()
        {
            var reservations = _context.Reservations.Include(r => r.Car).ToList();
            return View(reservations);
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Drivers()
        {
            var drivers = _context.Drivers.ToList();
            return View(drivers);
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Settings()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return NotFound("Invalid user ID.");
            }

            var user = _context.Users.FirstOrDefault(u => u.UsersId == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return View(user);
        }
    }
}
