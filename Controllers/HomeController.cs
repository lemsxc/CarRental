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

            var cars = _context.Cars.ToList();
            return View(cars);
        }

        [Authorize(Policy = "User")]
        public IActionResult Cars()
        {
            var cars = _context.Cars.ToList();
            return View(cars);
        }

        [Authorize(Policy = "User")]
        public IActionResult Reservations()
        {
            var reservations = _context.Reservations.Include(r => r.Car).ToList();
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
