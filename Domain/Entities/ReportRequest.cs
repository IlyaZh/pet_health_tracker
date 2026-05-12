namespace ArchieHealthTracker.Domain.Entities;

public record ReportRequest(
    long TelegramId,
    ReportCategory Category,
    MedicalEventType? MedicalEvent = null,
    HygieneEventType? HygieneEvent = null,
    SymptomType? SymptomType = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int? Limit = null
);