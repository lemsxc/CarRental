using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using CarRental.Models;
using CarRental.Services;

namespace UrbanDrive.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Login Page
        [HttpGet]
        public IActionResult Login() => View("~/Views/Auth/Login.cshtml");

        // Login User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Auth/Login.cshtml", model);

            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null || !VerifyPassword(model.Password, user.Password))
            {
                _logger.LogWarning($"Failed login attempt: {model.Email}");
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View("~/Views/Auth/Login.cshtml", model);
            }

            // Store user session
            HttpContext.Session.SetInt32("UserId", (int)user.UsersId);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role);

            _logger.LogInformation($"User logged in: {user.Email}");
            return RedirectToAction(user.Role == "Admin" ? "Dashboard" : "Home", "Page");
        }

        // Logout User
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // POST: Register User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = HashPassword(model.Password),
                Role = "Customer" // Default role (you can modify this)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // Hash password using PBKDF2
        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            byte[] hash = KeyDerivation.Pbkdf2(
                password, salt, KeyDerivationPrf.HMACSHA256, 100000, 32);

            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        // Verify password
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            byte[] computedHash = KeyDerivation.Pbkdf2(
                inputPassword, salt, KeyDerivationPrf.HMACSHA256, 100000, 32);

            return computedHash.SequenceEqual(storedHashBytes);
        }
    }
}
