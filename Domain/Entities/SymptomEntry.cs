using ArchieHealthTracker.Extensions.Interfaces;

namespace ArchieHealthTracker.Domain.Entities;

public class SymptomEntry : IHasCreatedAt
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public SymptomType Symptom { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public BotUser User { get; set; } = null!;
}