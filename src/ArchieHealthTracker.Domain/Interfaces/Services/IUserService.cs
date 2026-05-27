using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Interfaces.Services;

/// <summary>
/// Defines operations for managing bot users and their registration.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user or retrieves an existing one from the database.
    /// </summary>
    /// <param name="telegramId">The unique Telegram ID of the user.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="username">The user's Telegram username (optional).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple containing the <see cref="BotUser"/> entity and a boolean indicating if the user is new.</returns>
    Task<(BotUser User, bool IsNew)> RegisterUserAsync(long telegramId, string firstName, string? username,
        CancellationToken ct);
}
