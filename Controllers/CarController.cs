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

            TempData["AlertMessage"] = "Car Added Successfully!";
            TempData["AlertType"] = "success";

            // ✅ Redirect to Car List after successful insertion
            return RedirectToAction("Vehicles", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCar(Car car, IFormFile Image)
        {

            var existingCar = await _context.Cars.FindAsync(car.CarId);
            if (existingCar == null)
            {
                return NotFound();
            }

            // Update car fields
            existingCar.Brand = car.Brand;
            existingCar.Model = car.Model;
            existingCar.Category = car.Category;
            existingCar.Transmission = car.Transmission;
            existingCar.FuelType = car.FuelType;
            existingCar.FuelLevel = car.FuelLevel;
            existingCar.Seats = car.Seats;
            existingCar.PlateNumber = car.PlateNumber;
            existingCar.Mileage = car.Mileage;
            existingCar.RentPrice = car.RentPrice;
            existingCar.Condition = car.Condition;
            existingCar.Status = car.Status;

            // Handle image upload
            if (Image != null && Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "car_images");
                Directory.CreateDirectory(uploadsFolder); // make sure folder exists

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                // Update image path in DB
                existingCar.Image = "/car_images/" + uniqueFileName;
            }

            _context.Update(existingCar);
            await _context.SaveChangesAsync();

            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId);

            var admin = await _context.Users.FindAsync(adminId);
            if (admin != null)
            {
                string fullName = $"{admin.FirstName} {admin.LastName}";
                string description = $"Updated car: {car.Brand} {car.Model} ({car.Category}), Plate: {car.PlateNumber}";
                await _logsService.LogAsync("Update Car", description, fullName, admin.UsersId);
            }

            TempData["AlertMessage"] = "Car Updated Successfully!";
            TempData["AlertType"] = "success";

            return RedirectToAction("Vehicles", "Admin");
        }
    }
}
