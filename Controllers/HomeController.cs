using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CarRental.Services;
using CarRental.Models;
using System.Diagnostics;

namespace CarRental.Controllers;

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
        return View(cars);
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
        return View(cars);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
