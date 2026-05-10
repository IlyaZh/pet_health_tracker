using ArchieHealthTracker.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ArchieHealthTracker.Services;

public interface IUserSessionService
{
    public void SetUserState(Guid userId, UserSession session);
    public UserSession? GetCurrentState(Guid userId);
    public void ClearSession(Guid userId);
};

public class UserSessionService : IUserSessionService
{
    private readonly IMemoryCache _cache;

    public UserSessionService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void SetUserState(Guid userId, UserSession session)
    {
        _cache.Set(GetKey(userId), session, TimeSpan.FromMinutes(10));
    }

    public UserSession? GetCurrentState(Guid userId)
    {
        return _cache.TryGetValue(GetKey(userId), out UserSession? session) ? session : null;
    }

    public void ClearSession(Guid userId)
    {
        _cache.Remove(GetKey(userId));
    }

    private string GetKey(Guid id) => $"UserSession_{id}";
}