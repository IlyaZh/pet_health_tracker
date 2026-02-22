using ArchieHealthTracker.Entities;

namespace ArchieHealthTracker.Flows;

public enum ReportStep
{
    Type,
    Period,
    Confirm,
    Cancel
};

public class ReportFlowConfig
{
    private static readonly List<ReportStep> Flow = new()
    {
        ReportStep.Type,
        ReportStep.Period
    };

    public static ReportStep? GetNextStep(ReportStep currentStep)
    {
        var currentIndex = Flow.IndexOf(currentStep);
        if (currentIndex == -1 || currentIndex >= Flow.Count - 1) return null;
        return Flow[currentIndex + 1];
    }
}