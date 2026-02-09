using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Services;

public class HealthService : IHealthService
{
    private readonly IWeightRepository _weightRepository;
    private readonly IHygieneRepository _hygieneRepository;
    private readonly ISymptomRepository _symptomRepository;

    public HealthService(
        IWeightRepository weightRepository,
        IHygieneRepository hygieneRepository,
        ISymptomRepository symptomRepository
    )
    {
        _weightRepository = weightRepository;
        _hygieneRepository = hygieneRepository;
        _symptomRepository = symptomRepository;
    }

    private DateTime GetNowInUserTimeZone(string timeZoneId)
    {
        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone);
    }

    public async Task AddWeight(BotUser user, Weight weight, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(GetNowInUserTimeZone(user.TimeZoneId));

        var entry = new WeightEntry
        {
            Weight = weight,
            Date = today,
            UserId = user.Id
        };
        await _weightRepository.UpsertWeight(entry, ct);
    }

    public async Task AddHygiene(BotUser user, HygieneEventType action, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(GetNowInUserTimeZone(user.TimeZoneId));
        var entry = new HygieneEntry
        {
            Date = today,
            Event = action,
            UserId = user.Id
        };
        await _hygieneRepository.AddEvent(entry, ct);
    }

    public async Task AddSymptom(BotUser user, Symptom symptom, CancellationToken ct)
    {
        var entry = new SymptomEntry
        {
            Symptom = symptom.Type,
            Note = symptom.Note,
            UserId = user.Id
        };
        await _symptomRepository.AddSymptom(entry, ct);
    }
}