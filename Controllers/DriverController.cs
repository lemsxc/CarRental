using Microsoft.AspNetCore.Mvc;
using CarRental.Models;
using CarRental.Services;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace CarRental.Controllers
{
    public class DriverController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly ApplicationDbContext _context;
        private readonly ILogsService _logsService;

        public DriverController(IDriverService driverService, ILogsService logsService, ApplicationDbContext context)
        {
            _driverService = driverService;
            _context = context;
            _logsService = logsService;
        }

        public async Task<IActionResult> AddDriver(Driver driver)
        {
            if (driver == null)
            {
                return BadRequest("Invalid driver data.");
            }

            await _driverService.AddDriverAsync(driver);

            // ✅ Admin Logging
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId);

            var admin = await _context.Users.FindAsync(adminId); // Assumes _context injected
            if (admin != null)
            {
                string fullName = $"{admin.FirstName} {admin.LastName}";
                string description = $"Added a new driver: {driver.FirstName} {driver.LastName}, License: {driver.LicenseNumber}, Status: {driver.Status}";
                await _logsService.LogAsync("Add Driver", description, fullName, admin.UsersId);
            }

            return RedirectToAction("Drivers", "Admin");
        }


        public async Task<IActionResult> UpdateDriver(int id, Driver driver)
        {
            if (driver == null || id != driver.DriverId)
            {
                return BadRequest("Invalid driver data.");
            }

            await _driverService.UpdateDriverAsync(driver);

            // ✅ Admin Logging
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId);

            var admin = await _context.Users.FindAsync(adminId); // Assumes _context injected
            if (admin != null)
            {
                string fullName = $"{admin.FirstName} {admin.LastName}";
                string description = $"Updated driver details: {driver.FirstName} {driver.LastName}, License: {driver.LicenseNumber}, Status: {driver.Status}, Contact: {driver.ContactNumber}, Address: {driver.Address}";

                // Log the update action with relevant details
                await _logsService.LogAsync("Update Driver", description, fullName, admin.UsersId);
            }

            return RedirectToAction("Drivers", "Admin");
        }


        [HttpGet]
        [Route("api/drivers")]
        public async Task<IActionResult> FetchDrivers()
        {
            try
            {
                var drivers = await _driverService.GetAllDriversAsync();

                var driverList = drivers.Select(d => new
                {
                    driverId = d.DriverId,
                    name = $"{d.FirstName} {d.LastName}"
                });

                return Ok(driverList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
