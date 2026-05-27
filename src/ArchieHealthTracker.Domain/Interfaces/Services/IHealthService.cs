using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Interfaces.Services;

/// <summary>
/// Defines the main business logic for tracking pet health data.
/// </summary>
public interface IHealthService
{
    /// <summary>
    /// Records a new weight measurement for a pet.
    /// </summary>
    /// <param name="user">The user recording the weight.</param>
    /// <param name="weight">The weight value object.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddWeightAsync(BotUser user, Weight weight, CancellationToken ct);

    /// <summary>
    /// Records a hygiene-related activity (e.g., bath, paw cleaning).
    /// </summary>
    /// <param name="user">The user recording the activity.</param>
    /// <param name="action">The type of hygiene action.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddHygieneAsync(BotUser user, HygieneEventType action, CancellationToken ct);

    /// <summary>
    /// Records a new health symptom.
    /// </summary>
    /// <param name="user">The user recording the symptom.</param>
    /// <param name="symptom">The symptom details.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddSymptomAsync(BotUser user, Symptom symptom, CancellationToken ct);

    /// <summary>
    /// Records a medical event such as a vet visit or vaccination.
    /// </summary>
    /// <param name="user">The user recording the event.</param>
    /// <param name="medicalEvent">The medical event details.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddMedicalEventAsync(BotUser user, MedicalEvent medicalEvent, CancellationToken ct);

    /// <summary>
    /// Prepares data for generating a health report based on a specific request.
    /// </summary>
    /// <param name="request">The report request parameters (date range, type, etc.).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="ReportContext"/> containing the data needed for the report.</returns>
    Task<ReportContext> PrepareReportContextAsync(ReportRequest request, CancellationToken ct);
}
