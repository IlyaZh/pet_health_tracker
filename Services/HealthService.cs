using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Services;



public class HealthService : IHealthService
{
    // private readonly IHealthRepository _healthRepository;
    public Task AddWeight(BotUser user,  Weight weight)
    {
        throw new NotImplementedException();
    }
}