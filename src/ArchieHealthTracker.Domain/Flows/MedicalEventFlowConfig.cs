using ArchieHealthTracker.Domain.Entities;

namespace ArchieHealthTracker.Domain.Flows;

public enum MedicalEventStep
{
    Title,
    Dosage,
    Note,
    Confirm,
    Cancel
}

public class MedicalEventFlowConfig
{
    private static readonly Dictionary<MedicalEventType, List<MedicalEventStep>> Flows = new()
    {
        [MedicalEventType.Vaccination] = [MedicalEventStep.Title, MedicalEventStep.Dosage, MedicalEventStep.Note],
        [MedicalEventType.ParasiteTreatment] = [MedicalEventStep.Title, MedicalEventStep.Dosage, MedicalEventStep.Note],
        [MedicalEventType.Medication] = [MedicalEventStep.Title, MedicalEventStep.Dosage, MedicalEventStep.Note],
        [MedicalEventType.VetVisit] = [MedicalEventStep.Title, MedicalEventStep.Note],
    };

    public static MedicalEventStep? GetNextStep(MedicalEventType eventType, MedicalEventStep currentEventStep)
    {
        if (!Flows.TryGetValue(eventType, out var steps)) return null;
        var currentIndex = steps.IndexOf(currentEventStep);

        if (currentIndex == -1 || currentIndex >= steps.Count - 1) return null;

        return steps[currentIndex + 1];
    }
}