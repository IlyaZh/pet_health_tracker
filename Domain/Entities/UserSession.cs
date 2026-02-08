namespace ArchieHealthTracker.Entities;

public record struct UserSession
{
    public String CommandName { get; set; }
    public int MessageId { get; set; }
}