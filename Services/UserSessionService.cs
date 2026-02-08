using Microsoft.Extensions.Caching.Memory;

namespace ArchieHealthTracker.Services;

public interface IUserSessionService
{
    public void SetCommandState(Guid userId, string commandName);
    public string? GetCurrentCommand(Guid userId);
    public void ClearSession(Guid userId);
};

public class UserSessionService : IUserSessionService
{
    private readonly IMemoryCache _cache;
    
    public  UserSessionService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void SetCommandState(Guid userId, string commandName)
    {
        _cache.Set(GetKey(userId), commandName, TimeSpan.FromMinutes(10));
    }
    
    public string? GetCurrentCommand(Guid userId)
    {
        return _cache.TryGetValue(GetKey(userId), out string? commandName) ? commandName : null;
    }
    
    public void ClearSession(Guid userId)
    {
        _cache.Remove(GetKey(userId));
    }

    private string GetKey(Guid id) => $"UserSession_{id}";
    
}