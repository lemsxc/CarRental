using CarRental.Models;
using CarRental.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public class DriverService : IDriverService
    {
        private readonly ApplicationDbContext _context;

        public DriverService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all drivers
        public async Task<List<Driver>> GetAllDriversAsync()
        {
            return await _context.Drivers.ToListAsync();
        }

        // Get driver by ID
        public async Task<Driver> GetDriverByIdAsync(int driverId)
        {
            return await _context.Drivers.FindAsync(driverId);
        }

        // Add new driver
        public async Task AddDriverAsync(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
        }

        // Update driver details
        public async Task UpdateDriverAsync(Driver driver)
        {
            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
        }

        // Delete a driver
        public async Task DeleteDriverAsync(int driverId)
        {
            var driver = await _context.Drivers.FindAsync(driverId);
            if (driver != null)
            {
                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
            }
        }

        // Change driver status
        public async Task ChangeDriverStatusAsync(int driverId, string status)
        {
            var driver = await _context.Drivers.FindAsync(driverId);
            if (driver != null)
            {
                driver.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
