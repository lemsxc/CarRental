using CarRental.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Services
{
    public interface IPaymentService
    {
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<Payment> GetPaymentByIdAsync(int id);
        Task AddPaymentAsync(Payment payment);
    }
}