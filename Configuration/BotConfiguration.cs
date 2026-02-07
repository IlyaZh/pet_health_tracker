namespace ArchieHealthTracker.Configuration;

public class BotConfiguration
{
    public string Token { get; set; } = string.Empty;
    public HashSet<long> AllowedUsers { get; set; } = new();
}