namespace ArchieHealthTracker.Bot.Configuration;

public enum UpdateMode
{
    Polling,
    Webhook
}

public class BotConfiguration
{
    public UpdateMode UpdateMode { get; set; }
    public string Token { get; set; } = string.Empty;
    public HashSet<long> AllowedUsers { get; set; } = new();
    public string WebhookUrl { get; set; } = string.Empty;
    public string? SecretToken { get; set; }
}