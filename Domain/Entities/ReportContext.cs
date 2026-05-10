namespace ArchieHealthTracker.Domain.Entities;

public record ReportContext
{
    public DateTime? From { get; init; }
    public DateTime To { get; init; } = DateTime.UtcNow;
    public List<MedicalEventEntry>? MedicalEventsEntries { get; init; } = new();
    public List<WeightEntry>? WeightEntries { get; init; } = new();
    public List<SymptomEntry>? SymptomEntries { get; init; } = new();
    public List<HygieneEntry>? HygieneEntries { get; init; } = new();
}