using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.PipelineRegistration;

public class RegisterPipelineTests
{
    private static readonly Assembly[] _assemblies = [typeof(SampleContext).Assembly];

    [Fact]
    public void RegisterPipeline_ThrowsException_WhenContextImplementsIPipelineStep()
    {
        var name = typeof(SampleContextStepA).FullName ?? typeof(SampleContextStepA).Name;
        var services = new ServiceCollection();

        var ex = Should.Throw<ArgumentException>(() =>
        {
            services.RegisterPipeline<
                SampleContextStepA,
                IPipelineStep<SampleContextStepA>,
                IPipelineRunner<SampleContextStepA, IPipelineStep<SampleContextStepA>>>(_assemblies, ServiceLifetime.Transient, null);
        });

        ex.Message.ShouldStartWith(string.Format(PipeForge.PipelineRegistration.MessageInvalidContextType, name));
    }

    [Fact]
    public void RegisterPipeline_ReturnsEarly_WhenRunnerAlreadyRegistered()
    {
        var services = new ServiceCollection();
        services.AddTransient<ISampleContextRunner, SampleContextRunner>();

        services.RegisterPipeline<SampleContext, ISampleContextStep, ISampleContextRunner>(_assemblies, ServiceLifetime.Transient, null);

        services.Any(d => d.ServiceType == typeof(ISampleContextStep)).ShouldBeFalse();
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6, TestConstants.Filter1)]
    [InlineData(6, TestConstants.Filter2)]
    [InlineData(7, TestConstants.Filter1, TestConstants.Filter2)]
    public void RegisterPipeline_RegistersEverything(int expected, params string[]? filters)
    {
        var services = new ServiceCollection();

        services.RegisterPipeline<SampleContext, ISampleContextStep, ISampleContextRunner>(_assemblies, ServiceLifetime.Transient, filters);

        services.Count(d => d.ServiceType == typeof(ISampleContextStep)).ShouldBe(expected);
        services.Any(d => d.ServiceType == typeof(ISampleContextRunner)).ShouldBeTrue();
    }
}
