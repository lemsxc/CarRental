using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CarRental.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarController(ICarService carService, IWebHostEnvironment webHostEnvironment)
        {
            _carService = carService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAllCarsAsync();
            return View(cars);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Car car, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(car);
            }

            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    car.Image = $"Images/{uniqueFileName}";
                }

                car.Status = "Available";
                await _carService.AddCarAsync(car);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(car);
            }
        }

        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Car/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Car updatedCar, IFormFile ImageFile)
        {
            if (id != updatedCar.CarId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(updatedCar);
            }

            try
            {
                var existingCar = await _carService.GetCarByIdAsync(id);
                if (existingCar == null)
                {
                    return NotFound();
                }

                // If a new image is uploaded, save it
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Delete old image file if exists
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
                    updatedCar.Image = existingCar.Image; // Retain existing image if no new file is uploaded
                }

                await _carService.UpdateCarAsync(updatedCar);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(updatedCar);
            }
        }
    }
}
