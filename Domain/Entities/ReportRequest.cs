using Microsoft.Extensions.Configuration.UserSecrets;

namespace ArchieHealthTracker.Entities;

public record ReportRequest(
    long UserId,
    ReportCategory Category,
    MedicalEventType? MedicalEvent = null,
    HygieneEventType? HygieneEvent = null,
    SymptomType? SymptomType = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int? Limit  = null
    );