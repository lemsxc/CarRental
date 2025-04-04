using CarRental.Models;

namespace CarRental.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string token);
    }
}