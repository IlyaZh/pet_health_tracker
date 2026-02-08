using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Services;



public class HealthService : IHealthService
{
    private readonly IWeightRepository _weightRepository;
    private readonly IHygieneRepository _hygieneRepository;
    
    public HealthService(IWeightRepository weightRepository,  IHygieneRepository hygieneRepository)
    {
        _weightRepository = weightRepository;
        _hygieneRepository = hygieneRepository;
    }
    
    public async Task AddWeight(BotUser user,  Weight weight, CancellationToken ct)
    {
        var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId);
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone));
        
        var entry = new WeightEntry{
            Weight= weight,
            Date=  today,
            UserId= user.Id
            };
        await _weightRepository.UpsertWeight(entry, ct);
    }

    public async Task AddHygiene(BotUser user, HygieneEventType type, CancellationToken ct)
    {
        var entry = new HygieneEntry { };
        await _hygieneRepository.AddEvent(entry, ct);
    }
}