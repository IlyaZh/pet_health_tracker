using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IHealthService
{
        Task AddWeight(BotUser user, Weight weight, CancellationToken ct);
        Task AddHygiene(BotUser user, HygieneEventType type, CancellationToken ct);
}