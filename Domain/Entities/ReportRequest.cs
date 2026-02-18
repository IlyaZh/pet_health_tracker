namespace ArchieHealthTracker.Entities;

public enum ReportCategory
{
    All, Symptoms, MedicalEvents, Weight, Hygiene
};

public record ReportRequest(
    ReportCategory Category,
    MedicalEventType? MedicalEvent = null,
    HygieneEventType? HygieneEvent = null,
    SymptomType? SymptomType = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int? Limit  = null
    );