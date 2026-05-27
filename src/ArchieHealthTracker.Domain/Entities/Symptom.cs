namespace ArchieHealthTracker.Domain.Entities;

/// <summary>
/// Represents a health symptom entry for a pet.
/// </summary>
public class Symptom
{
    /// <summary>
    /// The type or category of the symptom.
    /// </summary>
    public SymptomType Type { get; set; }

    /// <summary>
    /// Optional notes or a more detailed description of the symptom.
    /// </summary>
    public string? Note { get; set; }
}
