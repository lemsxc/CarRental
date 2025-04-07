using CarRental.Models;

namespace CarRental.Services
{
    public interface ILogsService
    {
        Task LogAsync(string actionType, string description, string adminName, int? adminId = null);
    }
}