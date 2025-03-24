using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore; // Ensure this is included at the top
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
            // ✅ Check if user is logged in using cookie authentication
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Retrieve the user's role and ID from the claims (using cookie authentication)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // User ID
            var role = User.FindFirstValue(ClaimTypes.Role); // User Role

            if (userId == null || role != "User")
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
            // ✅ Check if user is logged in using cookie authentication
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Retrieve the user's role and ID from the claims (using cookie authentication)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // User ID
            var role = User.FindFirstValue(ClaimTypes.Role); // User Role

            if (userId == null || role != "User")
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

        [Authorize(Policy = "Admin")]
        public IActionResult Dashboard()
        {
            return View("~/Views/Admin/Dashboard.cshtml");
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Vehicles()
        {
            var cars = _context.Cars.ToList();
            return View("~/Views/Admin/CarList.cshtml", cars);
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Rentals()
        {
            var reservations = _context.Reservations
                .Include(r => r.Car) // Ensure Car data is loaded
                .ToList();

            return View("~/Views/Admin/Rentals.cshtml", reservations);
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Driver()
        {
            return View("~/Views/Admin/Driverlist.cshtml");
        }

        [Authorize(Policy = "Admin")]
        public IActionResult AddCar()
        {
            return View("~/Views/Admin/AddCars.cshtml");
        }

        [Authorize]
        public IActionResult Settings()
        {
            return View("~/Views/Admin/Settings.cshtml");
        }
    }
}
