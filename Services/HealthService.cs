using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;


namespace ArchieHealthTracker.Services;

public class HealthService : IHealthService
{
    private readonly IWeightRepository _weightRepository;
    private readonly IHygieneRepository _hygieneRepository;
    private readonly ISymptomRepository _symptomRepository;
    private readonly IMedicalEventRepository _medicalEventRepository;
    private static readonly int DefaultRequestLimit = 10;

    public HealthService(
        IWeightRepository weightRepository,
        IHygieneRepository hygieneRepository,
        ISymptomRepository symptomRepository,
        IMedicalEventRepository medicalEventRepository
    )
    {
        _weightRepository = weightRepository;
        _hygieneRepository = hygieneRepository;
        _symptomRepository = symptomRepository;
        _medicalEventRepository = medicalEventRepository;
    }

    private DateTime GetNowInUserTimeZone(string timeZoneId)
    {
        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone);
    }

    public async Task AddWeightAsync(BotUser user, Weight weight, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(GetNowInUserTimeZone(user.TimeZoneId));

        var entry = new WeightEntry
        {
            Weight = weight,
            Date = today,
            UserId = user.Id
        };
        await _weightRepository.AddOrUpdateWeight(entry, ct);
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
        await _hygieneRepository.AddEventAsync(entry, ct);
    }

    public async Task AddSymptomAsync(BotUser user, Symptom symptom, CancellationToken ct)
    {
        var entry = new SymptomEntry
        {
            Symptom = symptom.Type,
            Note = symptom.Note,
            UserId = user.Id
        };
        await _symptomRepository.AddSymptomAsync(entry, ct);
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
            Note =  medicalEvent.Note,
            Date = today,
        };
        await _medicalEventRepository.AddEventAsync(entry, ct);
    }

    public async Task<ReportContext> PrepareReportContextAsync(ReportRequest request, CancellationToken ct)
    {
        var baseParams = new QueryParams(request.Limit ?? DefaultRequestLimit, request.DateFrom, request.DateTo);
        var context = new ReportContext();

        if (request.Category is ReportCategory.All or ReportCategory.Hygiene)
        {
            context = context with
            {
                HygieneEntries = await _hygieneRepository.GetFilteredAsync(request.HygieneEvent, baseParams, ct)
            };
        }


        if (request.Category is ReportCategory.All or ReportCategory.Weight)
        {
            context = context with
            {
                WeightEntries = await _weightRepository.GetFilteredAsync(baseParams, ct)
            };
        }

        if (request.Category is ReportCategory.All or ReportCategory.MedicalEvent)
        {
            context = context with
            {
                MedicalEventsEntries =
                await _medicalEventRepository.GetFilteredAsync(request.MedicalEvent, baseParams, ct)
            };
        }

        if (request.Category is ReportCategory.All or ReportCategory.Symptom)
        {
            context = context with
            {
                SymptomEntries = await _symptomRepository.GetFilteredAsync(request.SymptomType, baseParams, ct)
            };
        }

        return context;
    }
}