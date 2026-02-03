using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Repositories;

public interface IUserRepository
{
    Task<BotUser> GetOrCreateUser(long telegramId, string firstName, string? username);
}