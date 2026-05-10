using ArchieHealthTracker.Extensions.Interfaces;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Domain.Entities;

public class HygieneEntry : IHasCreatedAt
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateOnly Date { get; set; }
    public HygieneEventType Event { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }

    public BotUser User { get; set; } = null!;
}