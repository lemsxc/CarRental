using CarRental.Models;
using CarRental.Services;

public class LogsService : ILogsService
{
    private readonly ApplicationDbContext _context;

    public LogsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string actionType, string description, string adminName, int? adminId = null)
    {
        var log = new Logs
        {
            ActionType = actionType,
            Description = description,
            AdminName = adminName,
            AdminId = adminId ?? 0,
            ActionDate = DateTime.Now
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
    }
}