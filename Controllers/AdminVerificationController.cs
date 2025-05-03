using Microsoft.AspNetCore.Mvc;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

public class AdminVerificationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogsService _logsService;

    public AdminVerificationController(ApplicationDbContext context, ILogsService logsService)
    {
        _context = context;
        _logsService = logsService;
    }

    [HttpPost]
    public async Task<IActionResult> UpdateVerification(int id, string status, string? adminRemarks)
    {
        var verification = _context.Verifications.Find(id);
        if (verification == null)
            return NotFound();

        verification.Status = status;
        verification.UpdatedAt = DateTime.Now;

        var user = _context.Users.FirstOrDefault(u => u.UsersId == verification.UserId);
        if (status == "Approved" && user != null)
        {
            user.IsVerified = true;
        }

        _context.SaveChanges();

        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId);
        var admin = await _context.Users.FindAsync(adminId);

        if (admin != null)
        {
            string fullName = $"{admin.FirstName} {admin.LastName}";
            string description = $"Application for Verification. Approved Verification ID: {verification.Id}.";
            await _logsService.LogAsync("Approved", description, fullName, admin.UsersId);
        }

        TempData["AlertMessage"] = "Verification request has been processed.";
        TempData["AlertType"] = "success";
        return RedirectToAction("Verifications", "Admin");
    }
}
