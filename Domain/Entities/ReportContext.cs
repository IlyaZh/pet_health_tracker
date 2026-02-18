namespace ArchieHealthTracker.Entities;

public record ReportContext
{
    public List<MedicalEventEntry>? MedicalEventsEntries { get; set; } = new();
    public List<WeightEntry>? WeightEntries { get; set; } = new();
    public List<SymptomEntry>? SymptomEntries { get; set; } = new();
    public List<HygieneEntry>? HygieneEntries { get; set; } = new();
}