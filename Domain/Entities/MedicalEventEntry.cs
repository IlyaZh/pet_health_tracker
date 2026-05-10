using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArchieHealthTracker.Extensions.Interfaces;

namespace ArchieHealthTracker.Domain.Entities;

public class MedicalEventEntry : IHasCreatedAt
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public BotUser User { get; set; } = null!;
    
    public MedicalEventType Type { get; set; }
    
    [Required, MaxLength(128)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Dosage { get; set; }

    public string? Note { get; set; }
    
    public DateOnly Date { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}