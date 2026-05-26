using ArchieHealthTracker.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ArchieHealthTracker.Application.Services;

public interface IUserSessionService
{
    public void SetUserState(Guid userId, UserSession session);
    public UserSession? GetCurrentState(Guid userId);
    public void ClearSession(Guid userId);
}

public class UserSessionService(IMemoryCache cache) : IUserSessionService
{
    public void SetUserState(Guid userId, UserSession session)
    {
        cache.Set(GetKey(userId), session, TimeSpan.FromMinutes(10));
    }

    public UserSession? GetCurrentState(Guid userId)
    {
        return cache.TryGetValue(GetKey(userId), out UserSession? session) ? session : null;
    }

    public void ClearSession(Guid userId)
    {
        cache.Remove(GetKey(userId));
    }

    private string GetKey(Guid id)
    {
        return $"UserSession_{id}";
    }
}