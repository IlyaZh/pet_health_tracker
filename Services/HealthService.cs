using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Services;



public class HealthService : IHealthService
{
    private readonly IWeightRepository _weightRepository;
    
    public HealthService(IWeightRepository weightRepository)
    {
        _weightRepository = weightRepository;
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
}