namespace ArchieHealthTracker.Entities;

public record ReportContext
{
    public DateTime? From { get; set; }
    public DateTime To { get; set; } = DateTime.UtcNow;
    public List<MedicalEventEntry>? MedicalEventsEntries { get; set; } = new();
    public List<WeightEntry>? WeightEntries { get; set; } = new();
    public List<SymptomEntry>? SymptomEntries { get; set; } = new();
    public List<HygieneEntry>? HygieneEntries { get; set; } = new();
}