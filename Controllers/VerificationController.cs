using Microsoft.AspNetCore.Mvc;
using CarRental.Models;
using CarRental.Services;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

public class VerificationController : Controller
{
    private readonly ApplicationDbContext _context;

    public VerificationController(ApplicationDbContext context)
    {
        _context = context;
    }

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

        string sanitizedLastName = lastName.Trim().Replace(" ", "_");
        string frontFileExt = Path.GetExtension(driversLicenseFrontFile.FileName);
        string backFileExt = Path.GetExtension(driversLicenseBackFile.FileName);
        string frontFileName = $"{sanitizedLastName}-front{frontFileExt}";
        string backFileName = $"{sanitizedLastName}-back{backFileExt}";

        string frontFilePath = Path.Combine(uploadFolder, frontFileName);
        string backFilePath = Path.Combine(uploadFolder, backFileName);

        using (var stream = new FileStream(frontFilePath, FileMode.Create))
        {
            driversLicenseFrontFile.CopyTo(stream);
        }

        using (var stream = new FileStream(backFilePath, FileMode.Create))
        {
            driversLicenseBackFile.CopyTo(stream);
        }

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
}
