using Telegram.Bot.Types;

namespace ArchieHealthTracker.Entities;

public class HygieneEntry
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateOnly Date { get; set; }
    public HygieneEventType Event { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    
    public BotUser User { get; set; } = null!;
}