using ArchieHealthTracker.Extensions.Interfaces;

namespace ArchieHealthTracker.Domain.Entities;

public class WeightEntry : IHasCreatedAt
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public Guid UserId { get; set; }

    public Weight Weight { get; set; }

    public DateOnly Date { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public BotUser User { get; set; } = null!;
}