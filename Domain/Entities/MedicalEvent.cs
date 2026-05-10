namespace ArchieHealthTracker.Domain.Entities;

public class MedicalEvent
{
    public MedicalEventType Type { get; set; }
    public required string Title { get; set; } 
    public string? Dosage  { get; set; }
    public string? Note  { get; set; }
}