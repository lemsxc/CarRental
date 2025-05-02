using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class RentalController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ILogsService _logsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public RentalController(IReservationService reservationService, IWebHostEnvironment webHostEnvironment, ILogsService logsService, ApplicationDbContext context)
        {
            _reservationService = reservationService;
            _webHostEnvironment = webHostEnvironment;
            _logsService = logsService;
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDone(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Car) // <-- This is the crucial part
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null || reservation.Car == null)
            {
                TempData["Error"] = "Reservation or car not found.";
                return RedirectToAction("Rentals", "Admin");
            }

            reservation.Status = "Done";
            reservation.ReturnDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId);
            var admin = await _context.Users.FindAsync(adminId);

            if (admin != null)
            {
                string fullName = $"{admin.FirstName} {admin.LastName}";
                string description = $"Marked rental as done. Car: {reservation.Car.Brand} {reservation.Car.Model}";
                await _logsService.LogAsync("Mark Done", description, fullName, admin.UsersId);
            }

            TempData["Success"] = "Rental marked as done.";
            return RedirectToAction("Rentals", "Admin");
        }

    }
}
