using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        ILogger<UserService> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<(BotUser User, bool IsNew)> RegisterUserAsync(long telegramId, string firstName, string? username,
        CancellationToken ct)
    {
        var user = await _userRepository.GetByTelegramIdAsync(telegramId, ct);
        if (user != null)
        {
            return (user, false);
        }
        
        _logger.LogInformation("[UserServer] new user creation");
        user = new BotUser
        {
            Id = Guid.CreateVersion7(),
            TelegramId = telegramId,
            FirstName = firstName,
            Username = username,
            TimeZoneId = "Central European Standard Time",
            CreatedAt = DateTime.UtcNow
        };
        await _userRepository.AddAsync(user, ct);

        return (user, true);
    }
}