using System.Reflection;
using PipeForge.Tests.Steps;
using PipeForge.Tests.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PipeForge.Tests;

public class PipelineTests
{
    private static readonly Assembly StepAssembly = typeof(StepA1).Assembly;

    [Fact]
    public void Discover_ReturnsNoDescriptors_WhenNoStepsExist()
    {
        var typeName = typeof(NotAContextWithSteps).FullName ?? typeof(NotAContextWithSteps).Name;
        var logger = new TestLogger();
        var descriptors = Pipeline.Discover<NotAContextWithSteps>(logger);

        descriptors.ShouldNotBeNull();
        descriptors.ShouldBeEmpty();

        logger.LogEntries.Count.ShouldBe(1);
        logger.LogEntries[0].Message.ShouldBe(string.Format(Pipeline.NoStepsFoundMessage, typeName));
    }

    [Fact]
    public void Discover_ReturnsDescriptors_WithFilter()
    {
        var descriptors1 = Pipeline.Discover<StepContext>(StepAssembly, "1").ToList();
        var descriptors2 = Pipeline.Discover<StepContext>(typeof(StepF), "2").ToList();

        descriptors1.ShouldNotBeNull();
        descriptors1.ShouldNotBeEmpty();
        descriptors1.Count().ShouldBe(4);

        descriptors2.ShouldNotBeNull();
        descriptors2.ShouldNotBeEmpty();
        descriptors2.Count().ShouldBe(4);

        descriptors1[0].ImplementationType.ShouldBe(typeof(StepA1));
        descriptors1[1].ImplementationType.ShouldBe(typeof(StepB1));
        descriptors1[2].ImplementationType.ShouldBe(typeof(StepC1));
        descriptors1[3].ImplementationType.ShouldBe(typeof(StepD));

        descriptors2[0].ImplementationType.ShouldBe(typeof(StepA2));
        descriptors2[1].ImplementationType.ShouldBe(typeof(StepC2));
        descriptors2[2].ImplementationType.ShouldBe(typeof(StepB2));
        descriptors2[3].ImplementationType.ShouldBe(typeof(StepD));
    }

    [Fact]
    public void Discover_ReturnsSteps_WithoutFilter()
    {
        var descriptors = Pipeline.Discover<StepContext>();

        descriptors.ShouldNotBeNull();
        descriptors.ShouldNotBeEmpty();
        descriptors.Count().ShouldBe(1);

        descriptors.First().ImplementationType.ShouldBe(typeof(StepD));
    }

    [Fact]
    public void Discover_OutputsRelevantLogs()
    {
        var typeName = typeof(StepContext).FullName ?? typeof(StepContext).Name;
        var logger = new TestLogger();
        _ = Pipeline.Discover<StepContext>([StepAssembly], "1", logger);

        logger.LogEntries.ShouldNotBeEmpty();
        logger.LogEntries.Count.ShouldBe(5);

        logger.ShouldContainMessage(string.Format(Pipeline.NumberStepsFoundMessage, 4, typeName), LogLevel.Debug);
        logger.ShouldContainMessage(string.Format(Pipeline.StepDiscoveredMessage, "PipeForge.Tests.Steps.StepA1", 1, 1, true), LogLevel.Debug);
        logger.ShouldContainMessage(string.Format(Pipeline.StepDiscoveredMessage, "PipeForge.Tests.Steps.StepB1", 2, 1, true), LogLevel.Debug);
        logger.ShouldContainMessage(string.Format(Pipeline.StepDiscoveredMessage, "PipeForge.Tests.Steps.StepC1", 3, 1, true), LogLevel.Debug);
        logger.ShouldContainMessage(string.Format(Pipeline.StepDiscoveredMessage, "PipeForge.Tests.Steps.StepD", 5, "(none)", true), LogLevel.Debug);
    }

    [Fact]
    public void Register_DoesNotRegister_IfPipelineRunnerIsAlreadyRegistered()
    {
        var logger = new TestLogger();
        var typeName = typeof(StepContext).FullName ?? typeof(StepContext).Name;

        var services = new ServiceCollection();
        services.AddSingleton<IPipelineRunner<StepContext>, PipelineRunner<StepContext>>();

        var descriptors = Pipeline.Discover<StepContext>("1");
        descriptors.ShouldNotBeEmpty();

        Pipeline.Register<StepContext>(services, descriptors, logger);

        logger.LogEntries.Count.ShouldBe(1);
        logger.ShouldContainMessage(string.Format(Pipeline.RunnerAlreadyRegisteredMessage, typeName), LogLevel.Debug);

        services.Any(s => s.ServiceType == typeof(IPipelineStep<StepContext>)).ShouldBeFalse();
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void Register_RegistersStepsAndRunner(ServiceLifetime lifetime)
    {
        var logger = new TestLogger();
        var runnerTypeName = typeof(IPipelineRunner<StepContext>).FullName ?? typeof(IPipelineRunner<StepContext>).Name;

        var descriptors = Pipeline.Discover<StepContext>("1");
        descriptors.ShouldNotBeEmpty();

        var services = new ServiceCollection();

        Pipeline.Register<StepContext>(services, descriptors, lifetime, logger);

        var stepTypeA = typeof(StepA1).FullName ?? typeof(StepA1).Name;
        var stepTypeB = typeof(StepB1).FullName ?? typeof(StepB1).Name;
        var stepTypeC = typeof(StepC1).FullName ?? typeof(StepC1).Name;
        var stepTypeD = typeof(StepD).FullName ?? typeof(StepD).Name;

        logger.LogEntries.Count.ShouldBe(5);
        logger.ShouldContainMessage(string.Format(Pipeline.StepRegistrationMessage, stepTypeA, lifetime), LogLevel.Debug);
        logger.ShouldContainMessage(string.Format(Pipeline.StepRegistrationMessage, stepTypeB, lifetime), LogLevel.Debug);
        logger.ShouldContainMessage(string.Format(Pipeline.StepRegistrationMessage, stepTypeC, lifetime), LogLevel.Debug);
        logger.ShouldContainMessage(string.Format(Pipeline.StepRegistrationMessage, stepTypeD, lifetime), LogLevel.Debug);
        logger.ShouldContainMessage(string.Format(Pipeline.RunnerRegistrationMessage, runnerTypeName, lifetime), LogLevel.Debug);
    }

    [Fact]
    public void SafeGetTypes_ReturnsTypes_WhenNoException()
    {
        var expected = new[] { typeof(string), typeof(int) };

        var result = Pipeline.SafeGetTypes(() => expected);

        result.ShouldBe(expected);
    }

    [Fact]
    public void SafeGetTypes_ReturnsNonNullTypes_WhenReflectionTypeLoadException()
    {
        var expected = new[] { typeof(string), null, typeof(int) };

        var ex = new ReflectionTypeLoadException(expected, new Exception[0]);

        var result = Pipeline.SafeGetTypes(() => throw ex);

        result.ShouldBe(new[] { typeof(string), typeof(int) });
    }

    [Fact]
    public void SafeGetTypes_ReturnsEmpty_WhenUnknownException()
    {
        var result = Pipeline.SafeGetTypes(() => throw new InvalidOperationException());

        result.ShouldBeEmpty();
    }

    private class NotAContextWithSteps { }
}
