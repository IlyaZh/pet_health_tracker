using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Repositories;
using ArchieHealthTracker.Domain.Interfaces.Services;

namespace ArchieHealthTracker.Application.Services;

/// <summary>
/// Implementation of <see cref="IHealthService"/> that manages pet health records
/// including weight, hygiene activities, symptoms, and medical events.
/// </summary>
public class HealthService(
    IWeightRepository weightRepository,
    IHygieneRepository hygieneRepository,
    ISymptomRepository symptomRepository,
    IMedicalEventRepository medicalEventRepository)
    : IHealthService
{
    private static readonly int DefaultRequestLimit = 10;

    public async Task AddWeightAsync(BotUser user, Weight weight, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(GetNowInUserTimeZone(user.TimeZoneId));

        var entry = new WeightEntry
        {
            Weight = weight,
            Date = today,
            UserId = user.Id
        };
        await weightRepository.AddOrUpdateWeight(entry, ct);
    }

    public async Task AddHygieneAsync(BotUser user, HygieneEventType action, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(GetNowInUserTimeZone(user.TimeZoneId));
        var entry = new HygieneEntry
        {
            Date = today,
            Event = action,
            UserId = user.Id
        };
        await hygieneRepository.AddEventAsync(entry, ct);
    }

    public async Task AddSymptomAsync(BotUser user, Symptom symptom, CancellationToken ct)
    {
        var entry = new SymptomEntry
        {
            Symptom = symptom.Type,
            Note = symptom.Note,
            UserId = user.Id
        };
        await symptomRepository.AddSymptomAsync(entry, ct);
    }

    public async Task AddMedicalEventAsync(BotUser user, MedicalEvent medicalEvent, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(GetNowInUserTimeZone(user.TimeZoneId));
        var entry = new MedicalEventEntry
        {
            UserId = user.Id,
            Type = medicalEvent.Type,
            Title = medicalEvent.Title,
            Dosage = medicalEvent.Dosage,
            Note = medicalEvent.Note,
            Date = today
        };
        await medicalEventRepository.AddEventAsync(entry, ct);
    }

    public async Task<ReportContext> PrepareReportContextAsync(ReportRequest request, CancellationToken ct)
    {
        var baseParams = new QueryParams(request.Limit ?? DefaultRequestLimit, request.DateFrom, request.DateTo);
        List<MedicalEventEntry>? medicalEventEntries = null;
        List<WeightEntry>? weightEntries = null;
        List<SymptomEntry>? symptomEntries = null;
        List<HygieneEntry>? hygieneEntries = null;

        if (request.Category is ReportCategory.All or ReportCategory.MedicalEvent)
            medicalEventEntries = await medicalEventRepository.GetFilteredAsync(request.MedicalEvent, baseParams, ct);

        if (request.Category is ReportCategory.All or ReportCategory.Weight)
            weightEntries = await weightRepository.GetFilteredAsync(baseParams, ct);

        if (request.Category is ReportCategory.All or ReportCategory.Symptom)
            symptomEntries = await symptomRepository.GetFilteredAsync(request.SymptomType, baseParams, ct);

        if (request.Category is ReportCategory.All or ReportCategory.Hygiene)
            hygieneEntries = await hygieneRepository.GetFilteredAsync(request.HygieneEvent, baseParams, ct);

        var context = new ReportContext() with
        {
            From = request.DateFrom,
            To = request.DateTo ?? DateTime.UtcNow,
            MedicalEventsEntries = medicalEventEntries,
            WeightEntries = weightEntries,
            SymptomEntries = symptomEntries,
            HygieneEntries = hygieneEntries
        };

        return context;
    }

    private DateTime GetNowInUserTimeZone(string timeZoneId)
    {
        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone);
    }
}
