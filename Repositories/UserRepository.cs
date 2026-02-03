using ArchieHealthTracker.Data;
using ArchieHealthTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly AppDbContext _context;
    
    public UserRepository(ILogger<UserRepository> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<BotUser> GetOrCreateUser(long telegramId, string firstName, string? username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);

        if (user != null)
        {
            return user;
        }

        user = new BotUser
        {
            Id = Guid.NewGuid(),
            TelegramId = telegramId,
            FirstName = firstName,
            Username = username,
            CreatedAt = DateTime.UtcNow,
            TimeZoneId = "Central European Standard Time"
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}