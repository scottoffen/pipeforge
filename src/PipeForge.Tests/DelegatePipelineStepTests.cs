using PipeForge.Tests.Steps;

namespace PipeForge.Tests;

public class DelegatePipelineStepTests
{
    [Fact]
    public void Constructor_ThrowsException_WhenActionIsNull()
    {
        var ex = Should.Throw<ArgumentNullException>(() =>
        {
            _ = new DelegatePipelineStep<SampleContext>(null!);
        });
    }

    [Fact]
    public async Task Execute_CallsAction_WhenInvoked()
    {
        var wasCalled = false;
        PipelineDelegate<SampleContext> next = (_, _) => Task.CompletedTask;

        var step = new DelegatePipelineStep<SampleContext>(async (context, d, ct) =>
        {
            wasCalled = true;
            await Task.CompletedTask;
        });

        await step.InvokeAsync(new SampleContext(), next, CancellationToken.None);
        wasCalled.ShouldBeTrue();
    }
}
