namespace ArchieHealthTracker.Domain.Entities;

public class HealthEvent
{
    public Guid Id { get; set; }
    public HealthEventType Type { get; set; }
    public DateTime EventDate { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AttachmentId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid BotUserId { get; set; }
    public BotUser BotUser { get; set; } = null!;
}