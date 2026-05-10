using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Repositories;

public interface IHealthService
{
    Task AddWeightAsync(BotUser user, Weight weight, CancellationToken ct);
    Task AddHygieneAsync(BotUser user, HygieneEventType action, CancellationToken ct);
    Task AddSymptomAsync(BotUser user, Symptom symptom, CancellationToken ct);
    Task AddMedicalEventAsync(BotUser user, MedicalEvent medicalEvent, CancellationToken ct);
    Task<ReportContext> PrepareReportContextAsync(ReportRequest request, CancellationToken ct);
}