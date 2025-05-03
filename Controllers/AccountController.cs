using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarRental.Models;
using CarRental.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace CarRental.Controllers
{
    [Authorize(Policy = "User")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int id))
            {
                TempData["AlertMessage"] = "Invalid user session.";
                TempData["AlertType"] = "error";
                return RedirectToAction("Login", "Auth");
            }

            var user = _context.Users.FirstOrDefault(u => u.UsersId == id);
            if (user == null)
            {
                TempData["AlertMessage"] = "User not found.";
                TempData["AlertType"] = "error";
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }

        // POST: /Account/UpdateProfile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User updatedUser)
        {

            var existingUser = await _context.Users.FindAsync(updatedUser.UsersId);
            if (existingUser == null)
            {
                TempData["AlertMessage"] = "User not found.";
                TempData["AlertType"] = "danger";
                return RedirectToAction("Settings", "Home");
            }

            // Update only editable fields
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.ContactNumber = updatedUser.ContactNumber;
            existingUser.Address = updatedUser.Address;

            try
            {
                await _context.SaveChangesAsync();

                TempData["AlertMessage"] = "Profile updated successfully.";
                TempData["AlertType"] = "success";

                return RedirectToAction("Settings", "Home");
            }
            catch (Exception)
            {
                TempData["AlertMessage"] = "An error occurred while updating the profile.";
                TempData["AlertType"] = "danger";

                return RedirectToAction("Settings", "Home", updatedUser);
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["AlertMessage"] = "All fields are required.";
                TempData["AlertType"] = "error";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["AlertMessage"] = "New password and confirmation do not match.";
                TempData["AlertType"] = "error";
                return View();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaim);
            var user = await _context.Users.FindAsync(userId);

            if (user == null || !VerifyPassword(oldPassword, user.Password))
            {
                TempData["AlertMessage"] = "Old password is incorrect.";
                TempData["AlertType"] = "error";
                return RedirectToAction("Settings", "Home");
            }

            user.Password = HashPassword(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Password changed successfully!";
            TempData["AlertType"] = "success";

            return RedirectToAction("Settings", "Home");
        }

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
