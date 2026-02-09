using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchieHealthTracker.Entities;

public class MedicalEventEntry
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public BotUser User { get; set; } = null!;
    
    public MedicalEventType Type { get; set; }
    
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    public DateOnly Date { get; set; }
    
    [MaxLength(50)]
    public string? Dosage { get; set; }
    
    public DateOnly? NextPlannedDate { get; set; }

    public string? Note { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}