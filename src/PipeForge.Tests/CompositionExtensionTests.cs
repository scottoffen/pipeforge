using PipeForge.Tests.Steps;
using PipeForge.Tests.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PipeForge.Tests;

public class CompositionExtensionTests
{
    [Fact]
    public async Task AddPipelineFor_RegistersAllSteps()
    {
        var context = new StepContext();
        var services = new ServiceCollection();

        services.AddScoped<ILoggerFactory, TestLoggerProvider>();
        services.AddPipelineFor<StepContext>("1");

        var provider = services.BuildServiceProvider();

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        loggerFactory.ShouldNotBeNull();

        var logger = loggerFactory.CreateLogger<CompositionExtensionTests>();
        logger.ShouldNotBeNull();

        var runner = provider.GetRequiredService<IPipelineRunner<StepContext>>();
        runner.ShouldNotBeNull();

        await runner.ExecuteAsync(context);
        var result = context.ToString();
        result.ShouldBe("A1,B1,C1,D0");
    }

    [Fact]
    public async Task AddPipelineStep_RegistersStepForPipelineRunnerUsage()
    {
        var context = new StepContext();
        var services = new ServiceCollection();

        services.AddScoped<ILoggerFactory, TestLoggerProvider>();
        services.AddPipelineStep<StepF>();
        services.AddTransient<IPipelineRunner<StepContext>, PipelineRunner<StepContext>>();

        var provider = services.BuildServiceProvider();
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        loggerFactory.ShouldNotBeNull();

        var logger = loggerFactory.CreateLogger<CompositionExtensionTests>();
        logger.ShouldNotBeNull();

        var runner = provider.GetRequiredService<IPipelineRunner<StepContext>>();
        runner.ShouldNotBeNull();

        await runner.ExecuteAsync(context);
        var result = context.ToString();
        result.ShouldBe("FX");
    }

    [Fact]
    public void AddPipelineStep_ThrowsException_WhenStepDoesNotImplementClosedGenericInterface()
    {
        var typeName = typeof(NotAPipelineStep).FullName ?? typeof(NotAPipelineStep).Name;
        var services = new ServiceCollection();

        var exception = Should.Throw<ArgumentException>(() => services.AddPipelineStep<NotAPipelineStep>());

        exception.Message.ShouldBe(string.Format(CompositionExtensions.ArgumentExceptionMessage, typeName));
    }

    [Fact]
    public void AddPipelineStep_ThrowsException_WhenDuplicateStepIsRegistered()
    {
        var services = new ServiceCollection();
        services.AddPipelineStep<StepF>();

        var typeName = typeof(StepF).FullName ?? typeof(StepF).Name;
        var exception = Should.Throw<InvalidOperationException>(() => services.AddPipelineStep<StepF>());

        exception.Message.ShouldBe(string.Format(CompositionExtensions.InvalidOperationExceptionMessage, typeName));
    }

    private class NotAPipelineStep : IPipelineStep { }

    private class NotAContextWithSteps { }
}
