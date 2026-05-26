using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Repositories;
using ArchieHealthTracker.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Application.Services;

public class UserService(
    IUserRepository userRepository,
    ILogger<UserService> logger)
    : IUserService
{
    public async Task<(BotUser User, bool IsNew)> RegisterUserAsync(long telegramId, string firstName, string? username,
        CancellationToken ct)
    {
        var user = await userRepository.GetByTelegramIdAsync(telegramId, ct);
        if (user != null) return (user, false);

        logger.LogInformation("[UserServer] new user creation");
        user = new BotUser
        {
            Id = Guid.CreateVersion7(),
            TelegramId = telegramId,
            FirstName = firstName,
            Username = username,
            TimeZoneId = "Central European Standard Time",
            CreatedAt = DateTime.UtcNow
        };
        await userRepository.AddAsync(user, ct);

        return (user, true);
    }
}
