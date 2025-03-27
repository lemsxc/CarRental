using CarRental.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public interface IDriverService
    {
        Task<List<Driver>> GetAllDriversAsync();
        Task<Driver> GetDriverByIdAsync(int driverId);
        Task AddDriverAsync(Driver driver);
        Task UpdateDriverAsync(Driver driver);
        Task DeleteDriverAsync(int driverId);
        Task ChangeDriverStatusAsync(int driverId, string status);
    }
}