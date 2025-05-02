using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarRental.Models;
using CarRental.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

    }
}
