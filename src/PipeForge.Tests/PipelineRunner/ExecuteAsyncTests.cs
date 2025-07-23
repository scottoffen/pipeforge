using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.PipelineRunner;

public class ExecuteAsyncTests
{
    private static readonly Assembly[] _assemblies = [typeof(SampleContext).Assembly];

    [Fact]
    public async Task ExecuteAsync_ThrowsException_WhenContextIsNull()
    {
        var services = new ServiceCollection();
        services.AddPipeline<SampleContext, ISampleContextStep, ISampleContextRunner>(_assemblies, [TestConstants.Filter1]);

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<ISampleContextRunner>();
        runner.ShouldNotBeNull();

        await Should.ThrowAsync<ArgumentNullException>(() => runner.ExecuteAsync(null!));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRunPipelineSuccessfully()
    {
        var services = new ServiceCollection();
        services.AddPipeline<SampleContext, ISampleContextStep, ISampleContextRunner>(_assemblies);

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<ISampleContextRunner>();
        runner.ShouldNotBeNull();

        var context = new SampleContext();
        await runner.ExecuteAsync(context);

        var result = context.ToString();
        result.ShouldBe("A,C,B,Z");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRunPipelineSuccessfully_WithShortCircuit()
    {
        var services = new ServiceCollection();
        services.AddPipeline<SampleContext, ISampleContextStep, ISampleContextRunner>(_assemblies, [TestConstants.Filter1]);

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<ISampleContextRunner>();
        runner.ShouldNotBeNull();

        var context = new SampleContext();
        await runner.ExecuteAsync(context);

        var result = context.ToString();
        result.ShouldBe("A,C,B,F");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRunPipelineSuccessfully_WithException()
    {
        var services = new ServiceCollection();
        services.AddPipeline<SampleContext, ISampleContextStep, ISampleContextRunner>(_assemblies, [TestConstants.Filter2]);

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<ISampleContextRunner>();
        runner.ShouldNotBeNull();

        var context = new SampleContext();

        var ex = await Should.ThrowAsync<PipelineExecutionException<SampleContext>>(() => runner.ExecuteAsync(context));

        ex.ShouldNotBeNull();
        ex.InnerException.ShouldBeOfType<NotImplementedException>();
        ex.StepName.ShouldBe("F");
        ex.StepOrder.ShouldBe(3);

        var result = context.ToString();
        result.ShouldBe("A,C,B");
    }
}
