using Microsoft.AspNetCore.Mvc;
using CarRental.Models;
using CarRental.Services;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

public class VerificationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly LogsService _logsService;

    public VerificationController(ApplicationDbContext context, LogsService logsService)
    {
        _context = context;
        _logsService = logsService;
    }

    // Submit verification request
    [HttpPost]
    public IActionResult SubmitVerification(int usersId, string lastName, IFormFile driversLicenseFrontFile, IFormFile driversLicenseBackFile)
    {
        if (driversLicenseFrontFile == null || driversLicenseBackFile == null ||
            driversLicenseFrontFile.Length == 0 || driversLicenseBackFile.Length == 0)
        {
            return BadRequest("Both front and back license images are required.");
        }

        string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "License");
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        // Normalize and generate unique file names
        string sanitizedLastName = lastName.Trim().Replace(" ", "_");
        string frontFileExt = Path.GetExtension(driversLicenseFrontFile.FileName);
        string backFileExt = Path.GetExtension(driversLicenseBackFile.FileName);
        string frontFileName = $"{sanitizedLastName}-front{frontFileExt}";
        string backFileName = $"{sanitizedLastName}-back{backFileExt}";

        string frontFilePath = Path.Combine(uploadFolder, frontFileName);
        string backFilePath = Path.Combine(uploadFolder, backFileName);

        // Save front image
        using (var stream = new FileStream(frontFilePath, FileMode.Create))
        {
            driversLicenseFrontFile.CopyTo(stream);
        }

        // Save back image
        using (var stream = new FileStream(backFilePath, FileMode.Create))
        {
            driversLicenseBackFile.CopyTo(stream);
        }

        // Save paths to DB
        var verification = new Verification
        {
            UserId = usersId,
            LicenseFrontPath = "/License/" + frontFileName,
            LicenseBackPath = "/License/" + backFileName,
            Status = "Pending",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Verifications.Add(verification);
        _context.SaveChanges();

        TempData["AlertMessage"] = "Verification request submitted successfully.";
        TempData["AlertType"] = "success";

        return RedirectToAction("Settings", "Home");
    }

    // Approve or Reject Verification
    // Approve or Reject Verification
    [HttpPost]
    public async Task<IActionResult> UpdateVerification(int id, string status, string? adminRemarks)
    {
        var verification = _context.Verifications.Find(id);
        if (verification == null)
            return NotFound();

        verification.Status = status;
        verification.UpdatedAt = DateTime.Now;

        string? adminName = null;
        int? adminId = null;

        // Get admin info for logging
        if (HttpContext.Session.GetInt32("AdminId") is int sessionAdminId)
        {
            var admin = _context.Users.Find(sessionAdminId);
            if (admin != null)
            {
                adminName = $"{admin.FirstName} {admin.LastName}";
                adminId = admin.UsersId;
            }
        }

        if (status == "Approved")
        {
            var user = _context.Users.FirstOrDefault(u => u.UsersId == verification.UserId);
            if (user != null)
            {
                user.IsVerified = true;
            }

            // 📝 Log the approval
            if (adminName != null)
            {
                string description = $"Approved license verification for user ID {verification.UserId}.";
                await _logsService.LogAsync("License Approval", description, adminName, adminId);
            }
        }

        _context.SaveChanges();

        TempData["AlertMessage"] = "Verification request has been processed.";
        TempData["AlertType"] = "success";
        return RedirectToAction("Verifications", "Admin");
    }
}
