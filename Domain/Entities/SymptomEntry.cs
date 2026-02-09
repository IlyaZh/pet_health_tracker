namespace ArchieHealthTracker.Entities;

public class SymptomEntry
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public SymptomType Symptom { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public BotUser User { get; set; } = null!;
}