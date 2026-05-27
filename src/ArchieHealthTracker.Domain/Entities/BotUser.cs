namespace ArchieHealthTracker.Domain.Entities;

/// <summary>
/// Represents a user interacting with the Telegram bot.
/// </summary>
public class BotUser
{
    /// <summary>
    /// Unique identifier for the user in the database.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique Telegram ID assigned to the user.
    /// </summary>
    public long TelegramId { get; set; }

    /// <summary>
    /// The first name of the user as provided by Telegram.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The Telegram username (optional).
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// The user's preferred time zone identifier (e.g., "UTC", "Europe/Moscow").
    /// </summary>
    public string TimeZoneId { get; set; } = "UTC";

    /// <summary>
    /// Date and time when the user was first registered in the system.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
