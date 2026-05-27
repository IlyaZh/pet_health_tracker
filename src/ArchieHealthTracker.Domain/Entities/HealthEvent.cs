namespace ArchieHealthTracker.Domain.Entities;

/// <summary>
/// Represents a generic health-related event for a pet.
/// </summary>
public class HealthEvent
{
    /// <summary>
    /// Unique identifier for the health event.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The category of the health event (e.g., Weight, Symptom, Hygiene).
    /// </summary>
    public HealthEventType Type { get; set; }

    /// <summary>
    /// The date and time when the event occurred.
    /// </summary>
    public DateTime EventDate { get; set; }

    /// <summary>
    /// The primary value of the event (e.g., weight in kg or symptom description).
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes or details about the event.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Identifier for an attached file or image (e.g., Telegram FileId).
    /// </summary>
    public string? AttachmentId { get; set; }

    /// <summary>
    /// Timestamp when the event record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Reference to the user who recorded the event.
    /// </summary>
    public Guid BotUserId { get; set; }

    /// <summary>
    /// Navigation property for the associated user.
    /// </summary>
    public BotUser BotUser { get; set; } = null!;
}
