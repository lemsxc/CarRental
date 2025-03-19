using CarRental.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car> GetCarByIdAsync(int id);
        Task AddCarAsync(Car car);
        Task UpdateCarAsync(Car car);
    }
}
