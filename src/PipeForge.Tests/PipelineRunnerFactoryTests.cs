using System.Reflection;
using Microsoft.Extensions.Logging;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests;

public class PipelineRunnerFactoryTests
{
    private readonly Assembly[] _assemblies = [typeof(PipelineRunnerFactoryTests).Assembly];
    private readonly ILogger _logger = new LoggerFactory().CreateLogger("Tests");


    public interface IGeneratedRunner : IPipelineRunner<SampleContext, ISampleContextStep> { }

    [Fact]
    public void Factory_Creates_Type_That_Implements_Interface_And_Inherits_Base()
    {
        var type = PipelineRunnerFactory.CreatePipelineRunner<SampleContext, ISampleContextStep, IGeneratedRunner>();

        type.ShouldNotBeNull();
        type.IsAbstract.ShouldBeFalse();
        type.IsInterface.ShouldBeFalse();
        type.IsGenericType.ShouldBeFalse();

        typeof(IGeneratedRunner).IsAssignableFrom(type).ShouldBeTrue();
        typeof(PipelineRunner<SampleContext, ISampleContextStep>).IsAssignableFrom(type).ShouldBeTrue();
    }
}
