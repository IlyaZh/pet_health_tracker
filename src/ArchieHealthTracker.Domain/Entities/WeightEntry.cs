namespace ArchieHealthTracker.Domain.Entities;

public class WeightEntry : IHasCreatedAt
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public Guid UserId { get; set; }

    public Weight Weight { get; set; }

    public DateOnly Date { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public BotUser User { get; init; } = null!;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}