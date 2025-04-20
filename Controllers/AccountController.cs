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
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                TempData["AlertMessage"] = "Please fix the errors and try again.";
                TempData["AlertType"] = "warning";
                return View("Profile", updatedUser);
            }

            var user = _context.Users.FirstOrDefault(u => u.UsersId == updatedUser.UsersId);
            if (user == null)
            {
                TempData["AlertMessage"] = "User not found.";
                TempData["AlertType"] = "error";
                return RedirectToAction("Login", "Auth");
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.ContactNumber = updatedUser.ContactNumber;
            user.Address = updatedUser.Address;

            _context.SaveChanges();

            TempData["AlertMessage"] = "Profile updated successfully!";
            TempData["AlertType"] = "success";
            return RedirectToAction("Profile");
        }
    }
}
