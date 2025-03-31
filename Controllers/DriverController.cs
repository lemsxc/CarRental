using Microsoft.AspNetCore.Mvc;
using CarRental.Models;
using CarRental.Services;
using System;
using System.Threading.Tasks;

namespace CarRental.Controllers
{
    public class DriverController : Controller
    {
        private readonly IDriverService _driverService;

        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        public async Task<IActionResult> AddDriver(Driver driver)
        {
            if (driver == null)
            {
                return BadRequest("Invalid driver data.");
            }

            await _driverService.AddDriverAsync(driver);
            return RedirectToAction("Drivers", "Admin");
        }

        public async Task<IActionResult> UpdateDriver(int id, Driver driver)
        {
            if (driver == null || id != driver.DriverId)
            {
                return BadRequest("Invalid driver data.");
            }

            await _driverService.UpdateDriverAsync(driver);
            return RedirectToAction("Drivers", "Admin");
        }

        // ✅ Fetch available drivers as JSON
        [HttpGet]
        [Route("api/drivers")]
        public async Task<IActionResult> FetchDrivers()
        {
            try
            {
                var drivers = await _driverService.GetAllDriversAsync();
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
