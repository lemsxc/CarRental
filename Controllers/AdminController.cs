using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace CarRental.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogsService _logsService;

        public AdminController(ApplicationDbContext context, ILogsService logsService)
        {
            _context = context;
            _logsService = logsService;
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
        public async Task<IActionResult> Dashboard(string filter = "daily")
        {
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;

            var payments = await _context.Payments.ToListAsync();

            var grouped = filter switch
            {
                "weekly" => payments.GroupBy(p => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                p.PaymentDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                                .Select(g => new
                                {
                                    Label = "Week " + g.Key,
                                    Total = g.Sum(p => p.Amount)
                                })
                                .OrderBy(x => x.Label)
                                .ToList(),

                "monthly" => payments.GroupBy(p => p.PaymentDate.ToString("MMMM yyyy"))
                                .Select(g => new
                                {
                                    Label = g.Key,
                                    Total = g.Sum(p => p.Amount)
                                })
                                .OrderBy(x => x.Label)
                                .ToList(),

                _ => payments.GroupBy(p => p.PaymentDate.ToString("yyyy-MM-dd"))
                                .Select(g => new
                                {
                                    Label = g.Key,
                                    Total = g.Sum(p => p.Amount)
                                })
                                .OrderBy(x => x.Label)
                                .ToList()
            };

            ViewBag.TotalRevenue = payments.Sum(p => p.Amount);
            ViewBag.TotalBookings = await _context.Reservations.Include(r => r.Payment).CountAsync(r => r.Payment != null);
            ViewBag.ActiveRentals = await _context.Reservations.CountAsync(r => r.Status == "Active");
            ViewBag.ConfirmedRentals = await _context.Reservations.CountAsync(r => r.Status == "Confirmed");
            ViewBag.CancelledRentals = await _context.Reservations.CountAsync(r => r.Status == "Cancelled");
            ViewBag.DoneRentals = await _context.Reservations.CountAsync(r => r.Status == "Done");
            ViewBag.Dates = grouped.Select(x => x.Label).ToList();
            ViewBag.RevenueData = grouped.Select(x => x.Total).ToList();
            ViewBag.SelectedFilter = filter;

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
            var reservations = _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.Driver)
                .Include(r => r.User)
                .Include(r => r.Payment)
                .ToList();

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

        [Authorize(Policy = "Admin")]
        public IActionResult Verifications()
        {
            var verifications = _context.Verifications
                .Include(v => v.User) // Load user details
                .ToList();

            return View(verifications);
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Logs()
        {
            var logs = _context.Logs
                .Include(r => r.User)
                .ToList();

            return View(logs);
        }
    }
}
