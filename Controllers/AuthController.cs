using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CarRental.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        // POST: Login User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["AlertMessage"] = "Invalid email or password.";
                TempData["AlertType"] = "error";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.Password))
            {
                TempData["AlertMessage"] = "Invalid email or password.";
                TempData["AlertType"] = "error";
                return View();
            }

            // ✅ Create User Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UsersId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role) // ✅ Assign role for authorization
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            // ✅ Sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            if (user.Role == "Admin")
            {
                TempData["AlertMessage"] = "Login successful! Welcome back.";
                TempData["AlertType"] = "success";
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                TempData["AlertMessage"] = "Login successful!";
                TempData["AlertType"] = "success";
                return RedirectToAction("Home", "Home");
            }
        }

        // GET: Register Page
        [HttpGet]
        public IActionResult Register() => View();

        // POST: Register User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                TempData["AlertMessage"] = "This email is already registered.";
                TempData["AlertType"] = "info";
                return View();
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = HashPassword(password),
                Role = "User" // ✅ Default role for new users
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Creating account successfully!";
            TempData["AlertType"] = "success";

            return RedirectToAction("Login");
        }

        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authResult.Succeeded)
            {
                TempData["AlertMessage"] = "Google login failed.";
                TempData["AlertType"] = "error";
                return RedirectToAction("Login");
            }

            var email = authResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = authResult.Principal.FindFirst(ClaimTypes.GivenName)?.Value ?? "Unknown";
            var lastName = authResult.Principal.FindFirst(ClaimTypes.Surname)?.Value ?? "Unknown";

            if (string.IsNullOrEmpty(email))
            {
                TempData["AlertMessage"] = "Google login failed: Email missing.";
                TempData["AlertType"] = "error";
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // Only set password if user is new
                string password = firstName + lastName;

                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = HashPassword(password), // Only set this if user is new
                    Role = "User",
                    IsVerified = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Create claims for login
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UsersId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["AlertMessage"] = "Logged in with Google successfully!";
            TempData["AlertType"] = "success";

            return user.Role == "Admin" ? RedirectToAction("Dashboard", "Admin") : RedirectToAction("Home", "Home");
        }



        // ✅ Logout User (Clear Auth Cookie)
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ✅ Hash Password using SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // ✅ Verify Password
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return HashPassword(enteredPassword) == storedHash;
        }
    }
}
