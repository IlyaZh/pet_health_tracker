using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;

namespace ArchieHealthTracker.Services;

public interface IUserService
{
    Task<(BotUser User, bool IsNew)> RegisterUserAsync(long telegramId, string firstName, string? username);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public  UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(BotUser User, bool IsNew)> RegisterUserAsync(long telegramId, string firstName, string? username)
    {
        var user = await _userRepository.GetByTelegramIdAsync(telegramId);
        if (user != null)
        {
            return (user, false);
        }

        user = new BotUser{
            Id = Guid.CreateVersion7(),
            TelegramId = telegramId,
            FirstName = firstName,
            Username = username,
            TimeZoneId = "Central European Standard Time",
            CreatedAt = DateTime.UtcNow
        };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
                
                return (user, true);
    }

}