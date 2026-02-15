using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Flows;

public enum MedicalStep { Title, Dosage, Note, Confirm }

public class MedicalEventFlowConfig
{
    private static readonly Dictionary<MedicalEventType, List<MedicalStep>> _flows = new()
    {
        [MedicalEventType.Vaccination] = new(){MedicalStep.Title, MedicalStep.Dosage, MedicalStep.Note},
        [MedicalEventType.ParasiteTreatment] = new(){MedicalStep.Title, MedicalStep.Dosage, MedicalStep.Note},
        [MedicalEventType.Medication] = new(){MedicalStep.Title, MedicalStep.Dosage, MedicalStep.Note},
        [MedicalEventType.VetVisit] = new(){MedicalStep.Title, MedicalStep.Note},
    };

    public static MedicalStep? GetNextStep(MedicalEventType eventType, MedicalStep currentStep)
    {
        var steps = _flows[eventType];
        var currentIndex = steps.IndexOf(currentStep);
        return currentIndex == -1 ? null : steps[currentIndex + 1];
    }
}