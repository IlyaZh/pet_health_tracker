namespace ArchieHealthTracker.Entities;

public class BotUser
{
    public Guid Id { get; set; }
    public long TelegramId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? Username { get; set; }

    public string TimeZoneId { get; set; } = "UTC";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
};