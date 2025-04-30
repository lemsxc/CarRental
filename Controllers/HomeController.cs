using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CarRental.Models;
using CarRental.Services;
using System.Diagnostics;
using System.Linq;


namespace CarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Policy = "User")]
        public IActionResult Home()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Or however your login route is set
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.FirstOrDefault(u => u.UsersId.ToString() == userId);
            ViewBag.IsVerified = user != null && user.IsVerified;

            var availableCars = _context.Cars
                .Where(c => c.Status == "Available")
                .ToList();

            return View(availableCars);
        }

        [Authorize(Policy = "User")]
        public IActionResult Cars()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.FirstOrDefault(u => u.UsersId.ToString() == userId);
            ViewBag.IsVerified = user != null && user.IsVerified;

            var availableCars = _context.Cars
                .Where(c => c.Status == "Available")
                .ToList();

            return View(availableCars);
        }

        [Authorize(Policy = "User")]
        public IActionResult Reservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.FirstOrDefault(u => u.UsersId.ToString() == userId);
            ViewBag.IsVerified = user != null && user.IsVerified;

            // 👇 Filter reservations to show only those made by the current user
            var reservations = _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.UsersId.ToString() == userId)
                .ToList();

            return View(reservations);
        }

        [Authorize(Policy = "User")]
        public IActionResult Settings()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return NotFound("Invalid user ID.");
            }

            var user = _context.Users.FirstOrDefault(u => u.UsersId == userId);
            ViewBag.IsVerified = user != null && user.IsVerified;

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
