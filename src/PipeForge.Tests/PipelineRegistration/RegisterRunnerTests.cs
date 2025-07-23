using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.PipelineRegistration;

public class RegisterRunnerTests
{
    private static readonly Assembly[] Assemblies = [typeof(ISampleContextStep).Assembly];

    [Fact]
    public void RegisterRunner_ReturnsTrue_WhenRegistering_DefaultInterfaces()
    {
        var services = new ServiceCollection();

        var result = services.RegisterRunner<
            SampleContext,
            IPipelineStep<SampleContext>,
            IPipelineRunner<SampleContext, IPipelineStep<SampleContext>>>(Assemblies, ServiceLifetime.Transient, null);

        result.ShouldBeTrue();

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<IPipelineRunner<SampleContext, IPipelineStep<SampleContext>>>();
        runner.ShouldNotBeNull();
        runner.ShouldBeOfType<DefaultPipelineRunner<SampleContext, IPipelineStep<SampleContext>>>();
    }

    [Fact]
    public void RegisterRunner_ReturnsTrue_WhenRegistering_DefaultSimpleInterfaces()
    {
        var services = new ServiceCollection();

        var result = services.RegisterRunner<
            SampleContext,
            IPipelineStep<SampleContext>,
            IPipelineRunner<SampleContext>>(Assemblies, ServiceLifetime.Transient, null);

        result.ShouldBeTrue();

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<IPipelineRunner<SampleContext>>();
        runner.ShouldNotBeNull();
        runner.ShouldBeOfType<DefaultPipelineRunner<SampleContext>>();
    }

    [Fact]
    public void RegisterRunner_ReturnsTrue_WhenRegistering_CustomStepInterface()
    {
        var services = new ServiceCollection();

        var result = services.RegisterRunner<
            SampleContext,
            ISampleContextStep,
            IPipelineRunner<SampleContext, ISampleContextStep>>(Assemblies, ServiceLifetime.Transient, null);

        result.ShouldBeTrue();

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<IPipelineRunner<SampleContext, ISampleContextStep>>();
        runner.ShouldNotBeNull();
        runner.ShouldBeOfType<DefaultPipelineRunner<SampleContext, ISampleContextStep>>();
    }

    [Fact]
    public void RegisterRunner_ReturnsTrue_WhenRegistering_CustomRunnerInterface()
    {
        var services = new ServiceCollection();

        var result = services.RegisterRunner<
            SampleContext,
            ISampleContextStep,
            ISampleContextRunner>(Assemblies, ServiceLifetime.Transient, null);

        result.ShouldBeTrue();

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<ISampleContextRunner>();
        runner.ShouldNotBeNull();
        runner.ShouldBeOfType<SampleContextRunner>();
    }

    [Fact]
    public void RegisterRunner_ReturnsFalse_WhenRunnerIsAlreadyRegisters()
    {
        var services = new ServiceCollection();
        services.AddTransient<IPipelineRunner<SampleContext, IPipelineStep<SampleContext>>, PipelineRunner<SampleContext, IPipelineStep<SampleContext>>>();

        var result = services.RegisterRunner<
            SampleContext,
            IPipelineStep<SampleContext>,
            IPipelineRunner<SampleContext, IPipelineStep<SampleContext>>>(Assemblies, ServiceLifetime.Transient, null);

        result.ShouldBeFalse();
    }

    [Fact]
    public void RegisterRunner_Throws_WhenNoRunnerImplementationFound()
    {
        var runnerTypeName = typeof(INotImplementedRunner).FullName ?? typeof(INotImplementedRunner).Name;
        var services = new ServiceCollection();

        var ex = Should.Throw<InvalidOperationException>(() =>
        {
            services.RegisterRunner<
                SampleContext,
                ISampleContextStep,
                INotImplementedRunner>(Assemblies, ServiceLifetime.Transient, null);
        });

        ex.Message.ShouldBe(string.Format(PipeForge.PipelineRegistration.MessageRunnerImplementationNotFound, runnerTypeName));
    }
}
