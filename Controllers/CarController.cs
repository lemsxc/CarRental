using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CarRental.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly ILogsService _logsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public CarController(ICarService carService, IWebHostEnvironment webHostEnvironment, ILogsService logsService, ApplicationDbContext context)
        {
            _carService = carService;
            _webHostEnvironment = webHostEnvironment;
            _logsService = logsService;
            _context = context;
        }

        // POST: Add Car
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            string Brand, string Model, string Category, string Transmission, string FuelType,
            string PlateNumber, int Mileage, float RentPrice, int FuelLevel,
            string Condition, string Status, IFormFile Image)
        {
            // ✅ Validate Required Fields
            if (string.IsNullOrWhiteSpace(Brand) || string.IsNullOrWhiteSpace(Model) || string.IsNullOrWhiteSpace(PlateNumber))
            {
                ViewBag.Error = "Brand, Model, and Plate Number are required.";
                return View("~/Views/Admin/AddCar.cshtml");
            }

            string imagePath = null;

            // ✅ Handle Image Upload
            if (Image != null && Image.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(Image.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                imagePath = $"Images/{uniqueFileName}";
            }

            // ✅ Create New Car Object
            var car = new Car
            {
                Brand = Brand,
                Model = Model,
                Category = Category,
                Transmission = Transmission,
                PlateNumber = PlateNumber,
                Mileage = Mileage,
                RentPrice = RentPrice,
                FuelType = FuelType,
                FuelLevel = FuelLevel,
                Condition = Condition,
                Status = Status,
                Image = imagePath
            };

            // ✅ Insert into Database
            await _carService.AddCarAsync(car);

            // ✅ Admin Logging
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId);

            var admin = await _context.Users.FindAsync(adminId); // Assumes _context injected
            if (admin != null)
            {
                string fullName = $"{admin.FirstName} {admin.LastName}";
                string description = $"Added a new car: {Brand} {Model} ({Category})";
                await _logsService.LogAsync("Add Car", description, fullName, admin.UsersId);
            }

            // ✅ Redirect to Car List after successful insertion
            return RedirectToAction("Vehicles", "Admin");
        }

        // Update Car (instead of Edit)
        public async Task<IActionResult> Update(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View("EditCar", car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Car updatedCar, IFormFile ImageFile)
        {
            if (id != updatedCar.CarId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("EditCar", updatedCar);
            }

            try
            {
                var existingCar = await _carService.GetCarByIdAsync(id);
                if (existingCar == null)
                {
                    return NotFound();
                }

                if (ImageFile != null && ImageFile.Length > 0 && ImageFile.ContentType.Contains("image"))
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingCar.Image))
                    {
                        string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingCar.Image);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    updatedCar.Image = $"Images/{uniqueFileName}";
                }
                else
                {
                    updatedCar.Image = existingCar.Image;
                }

                // Update Car details
                await _carService.UpdateCarAsync(updatedCar);

                // ✅ Admin Logging for Update
                int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId);

                var admin = await _context.Users.FindAsync(adminId);
                if (admin != null)
                {
                    string fullName = $"{admin.FirstName} {admin.LastName}";
                    string description = $"Updated car details: {updatedCar.Brand} {updatedCar.Model}, Plate: {updatedCar.PlateNumber}";
                    await _logsService.LogAsync("Update Car", description, fullName, admin.UsersId);
                }

                return RedirectToAction("Vehicles", "Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return RedirectToAction("Vehicles", "Admin");
            }
        }
    }
}
