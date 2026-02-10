namespace ArchieHealthTracker.Entities;

public class MedicalEvent
{
    public MedicalEventType type { get; set; }
    public required string Title { get; set; } 
    public DateTime Date { get; set; }
    public string? Dosage  { get; set; }
    public string? Note  { get; set; }
}