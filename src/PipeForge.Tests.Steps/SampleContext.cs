namespace PipeForge.Tests.Steps;

/// <summary>
/// Sample context for testing pipeline steps.
/// This context allows you to:
///     <list type="bullet">
///         <item>Track pipeline progress via AddStep()</item>
///         <item>Print step execution history using ToString()</item>
///         <item>Assert how many steps ran using StepCount</item>
///         <item>Simulate errors by passing null or empty step names</item>
///     </list>
/// </summary>
public class SampleContext
{
    public readonly List<string> Steps = [];

    public void AddStep(string stepName)
    {
        if (string.IsNullOrWhiteSpace(stepName))
        {
            throw new ArgumentException("Step name cannot be null or whitespace.", nameof(stepName));
        }

        Steps.Add(stepName);
    }

    public int StepCount => Steps.Count;

    public override string ToString()
    {
        return string.Join(",", Steps);
    }
}
