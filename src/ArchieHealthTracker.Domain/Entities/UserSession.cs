namespace ArchieHealthTracker.Domain.Entities;

public class UserSession
{
    public string CommandName { get; set; } = string.Empty;
    public int MessageId { get; set; }
    public Dictionary<string, string> Metadata { get; init; } = new();
}