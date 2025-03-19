using CarRental.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Car> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task AddCarAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(Car car)
        {
            var existingCar = await _context.Cars.FindAsync(car.CarId);
            if (existingCar != null)
            {
                existingCar.Brand = car.Brand;
                existingCar.Model = car.Model;
                existingCar.Image = car.Image;
                existingCar.RentPrice = car.RentPrice;
                existingCar.Category = car.Category;
                existingCar.Transmission = car.Transmission;
                existingCar.FuelType = car.FuelType;
                existingCar.FuelLevel = car.FuelLevel;
                existingCar.PlateNumber = car.PlateNumber;
                existingCar.Mileage = car.Mileage;
                existingCar.Condition = car.Condition;
                existingCar.Status = car.Status;

                _context.Cars.Update(existingCar);
                await _context.SaveChangesAsync();
            }
        }
    }
}
