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

            // Retrieve all cars from the database
            var cars = _context.Cars.ToList(); // Assuming your DbSet<Car> is named 'Cars'

            // Pass the cars to the view
            return View("~/Views/Home/Home.cshtml", cars);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View("~/Views/Admin/Dashboard.cshtml");
        }

        public IActionResult CarRental()
        {
            return View();
        }

        public IActionResult Driver()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}
