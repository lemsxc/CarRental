using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarRental.Services;
using CarRental.Models;

namespace CarRental.Controllers
{
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject the database context into the controller
        public PageController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Home()
        {
            // ✅ Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the user's role from the database (assuming you have a User table with a Role field)
            var user = _context.Users.FirstOrDefault(u => u.UsersId == userId);
            if (user == null || user.Role != "User")
            {
                // If the user doesn't exist or doesn't have the "User" role, redirect or show an error page
                return RedirectToAction("AccessDenied", "Account");
            }

            // Retrieve all cars from the database
            var cars = _context.Cars.ToList(); // Assuming your DbSet<Car> is named 'Cars'

            // Pass the cars to the view
            return View("~/Views/Home/Home.cshtml", cars);
        }

        public IActionResult Cars()
        {
            // ✅ Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the user's role from the database (assuming you have a User table with a Role field)
            var user = _context.Users.FirstOrDefault(u => u.UsersId == userId);
            if (user == null || user.Role != "User")
            {
                // If the user doesn't exist or doesn't have the "User" role, redirect or show an error page
                return RedirectToAction("AccessDenied", "Account");
            }

            // Retrieve all cars from the database
            var cars = _context.Cars.ToList(); // Assuming your DbSet<Car> is named 'Cars'

            // Pass the cars to the view
            return View("~/Views/Home/Cars.cshtml", cars);
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "Admin")] // Restrict only for Admins
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Policy = "Admin")] // Only Admins can manage car rentals
        public IActionResult CarRental()
        {
            return View();
        }

        [Authorize(Policy = "Admin")] // Only Admins can manage drivers
        public IActionResult Driver()
        {
            return View();
        }

        [Authorize] // All authenticated users can access settings
        public IActionResult Settings()
        {
            return View();
        }
    }
}
