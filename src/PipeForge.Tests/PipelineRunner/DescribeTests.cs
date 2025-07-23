using Microsoft.Extensions.DependencyInjection;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.PipelineRunner;

public class DescribeTests
{
    private interface INamedStep : IPipelineStep<SampleContext> { }

    private class NamedStep : INamedStep
    {
        public string? Description { get; }

        public bool MayShortCircuit { get; }

        public string Name { get; }

        public string? ShortCircuitCondition { get; }

        public NamedStep(string name, string? description = null, bool shortCircuiting = false, string? condition = null)
        {
            Name = name;
            Description = description;
            MayShortCircuit = shortCircuiting;
            ShortCircuitCondition = condition;
        }

        public Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private class NamedStep1 : NamedStep
    {
        public static readonly string StepName = Guid.NewGuid().ToString();
        public static readonly string StepDescription = Guid.NewGuid().ToString();
        public static readonly string Condition = Guid.NewGuid().ToString();
        public static bool ShortCircuiting = true;

        public NamedStep1() : base(StepName, StepDescription, ShortCircuiting, Condition)
        {
        }
    }

    private class NamedStep2 : NamedStep
    {
        public static readonly string StepName = Guid.NewGuid().ToString();
        public static readonly string StepDescription = Guid.NewGuid().ToString();
        public static bool ShortCircuiting = false;

        public NamedStep2() : base(StepName, StepDescription, ShortCircuiting, null)
        {
        }
    }

    private class NamedStep3 : NamedStep
    {
        public static readonly string StepName = Guid.NewGuid().ToString();
        public static readonly string StepDescription = Guid.NewGuid().ToString();
        public static bool ShortCircuiting = false;

        public NamedStep3() : base(StepName, StepDescription, ShortCircuiting, null)
        {
        }
    }

    [Fact]
    public void Describe_ReturnsStepDescriptions_InOrderAndWithMetadata()
    {
        var services = new ServiceCollection();
        services.AddPipelineStep<NamedStep1, INamedStep>();
        services.AddPipelineStep<NamedStep2, INamedStep>();
        services.AddPipelineStep<NamedStep3, INamedStep>();
        services.RegisterRunner<SampleContext, INamedStep, IPipelineRunner<SampleContext, INamedStep>>([typeof(INamedStep).Assembly], ServiceLifetime.Transient, null);

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<IPipelineRunner<SampleContext, INamedStep>>();

        var json = runner.Describe();

        json.ShouldContain($"\"Name\": \"{NamedStep1.StepName}\"");
        json.ShouldContain($"\"Name\": \"{NamedStep2.StepName}\"");
        json.ShouldContain($"\"Name\": \"{NamedStep3.StepName}\"");

        json.ShouldContain("\"MayShortCircuit\": true");
        json.ShouldContain($"\"ShortCircuitCondition\": \"{NamedStep1.Condition}\"");

        json.IndexOf(NamedStep1.StepName).ShouldBeLessThan(json.IndexOf(NamedStep2.StepName));
        json.IndexOf(NamedStep2.StepName).ShouldBeLessThan(json.IndexOf(NamedStep3.StepName));
    }
}
