using System.Reflection;
using PipeForge.Extensions;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Extensions;

public class AssemblyExtensionsTests
{
    private static readonly Assembly[] _assemblies = [typeof(SampleContext).Assembly];

    [Fact]
    public void FindClosedImplementationsOf_ThrowsException_WhenTypeIsNotAnInterface()
    {
        var ex = Should.Throw<ArgumentException>(() =>
        {
            _assemblies.FindClosedImplementationsOf<string>();
        });

        ex.Message.ShouldStartWith(string.Format(PipeForge.Extensions.AssemblyExtensions.MessageNotAnInterface, typeof(string).FullName));
    }

    [Fact]
    public void FindClosedImplementationsOf_ReturnsAllClosedImplementations_ForValidInterface()
    {
        var result = _assemblies.FindClosedImplementationsOf<IPipelineStep<SampleContext>>();
        result.ShouldNotBeEmpty();

        result.ShouldSatisfyAllConditions
        (
            () => result.ShouldContain(t => t == typeof(SampleContextStepA)),
            () => result.ShouldNotContain(t => t == typeof(SampleContextStep)),
            () => result.ShouldNotContain(t => t == typeof(ISampleContextStep)),
            () => result.ShouldNotContain(t => t == typeof(IDisposablePipelineStep)),
            () => result.ShouldNotContain(t => t == typeof(OpenGenericPipelineStep<>)),
            () => result.ShouldNotContain(t => t == typeof(GenericPipelineStep<>)),
            () => result.ShouldNotContain(t => t == typeof(IGenericPipelineStep<>))
        );
    }

    [Fact]
    public void FindClosedImplementationsOf_ReturnsAllClosedImplementations_ForValidSpecificInterface()
    {
        var result = _assemblies.FindClosedImplementationsOf<ISampleContextStep>();
        result.ShouldNotBeEmpty();

        result.ShouldSatisfyAllConditions
        (
            () => result.ShouldContain(t => t == typeof(SampleContextStepA)),
            () => result.ShouldNotContain(t => t == typeof(SampleContextStep)),
            () => result.ShouldNotContain(t => t == typeof(ISampleContextStep)),
            () => result.ShouldNotContain(t => t == typeof(IDisposablePipelineStep)),
            () => result.ShouldNotContain(t => t == typeof(OpenGenericPipelineStep<>)),
            () => result.ShouldNotContain(t => t == typeof(ClosedGenericPipelineStep)),
            () => result.ShouldNotContain(t => t == typeof(GenericPipelineStep<>)),
            () => result.ShouldNotContain(t => t == typeof(IGenericPipelineStep<>))
        );
    }

    [Fact]
    public void GetDescriptorsFor_ReturnsEmpty_WhenNoStepsFound()
    {
        var result = _assemblies.GetDescriptorsFor<IPipelineStep<string>>(null);
        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetDescriptorsFor_ReturnsIEnumerable_ForValidStepInterface()
    {
        var result = _assemblies.GetDescriptorsFor<IPipelineStep<SampleContext>>(null).ToList();
        result.ShouldNotBeEmpty();

        result.ShouldSatisfyAllConditions
        (
            () => result.Count().ShouldBe(5),
            () => result[0].ImplementationType.ShouldBe(typeof(ClosedGenericPipelineStep)),
            () => result[1].ImplementationType.ShouldBe(typeof(SampleContextStepA)),
            () => result[2].ImplementationType.ShouldBe(typeof(SampleContextStepC)),
            () => result[3].ImplementationType.ShouldBe(typeof(SampleContextStepB)),
            () => result[4].ImplementationType.ShouldBe(typeof(SampleContextStepZ))
        );
    }

    [Fact]
    public void GetDescriptorsFor_ReturnsIEnumerable_ForValidCustomStepInterface_WithNoFilters()
    {
        var result = _assemblies.GetDescriptorsFor<ISampleContextStep>(null).ToList();
        result.ShouldNotBeEmpty();

        result.ShouldSatisfyAllConditions
        (
            () => result.Count().ShouldBe(4),
            () => result[0].ImplementationType.ShouldBe(typeof(SampleContextStepA)),
            () => result[1].ImplementationType.ShouldBe(typeof(SampleContextStepC)),
            () => result[2].ImplementationType.ShouldBe(typeof(SampleContextStepB)),
            () => result[3].ImplementationType.ShouldBe(typeof(SampleContextStepZ))
        );
    }

    [Fact]
    public void GetDescriptorsFor_ReturnsIEnumerable_ForValidCustomStepInterface_WithSingleFilter1()
    {
        var result = _assemblies.GetDescriptorsFor<ISampleContextStep>([TestConstants.Filter1]).ToList();
        result.ShouldNotBeEmpty();

        result.ShouldSatisfyAllConditions
        (
            () => result.Count().ShouldBe(6),
            () => result[0].ImplementationType.ShouldBe(typeof(SampleContextStepA)),
            () => result[1].ImplementationType.ShouldBe(typeof(SampleContextStepC)),
            () => result[2].ImplementationType.ShouldBe(typeof(SampleContextStepB)),
            () => result[3].ImplementationType.ShouldBe(typeof(SampleContextStepF1)),
            () => result[4].ImplementationType.ShouldBe(typeof(SampleContextStepM)),
            () => result[5].ImplementationType.ShouldBe(typeof(SampleContextStepZ))
        );
    }

    [Fact]
    public void GetDescriptorsFor_ReturnsIEnumerable_ForValidCustomStepInterface_WithSingleFilter2()
    {
        var result = _assemblies.GetDescriptorsFor<ISampleContextStep>([TestConstants.Filter2]).ToList();
        result.ShouldNotBeEmpty();

        result.ShouldSatisfyAllConditions
        (
            () => result.Count().ShouldBe(6),
            () => result[0].ImplementationType.ShouldBe(typeof(SampleContextStepA)),
            () => result[1].ImplementationType.ShouldBe(typeof(SampleContextStepC)),
            () => result[2].ImplementationType.ShouldBe(typeof(SampleContextStepB)),
            () => result[3].ImplementationType.ShouldBe(typeof(SampleContextStepF2)),
            () => result[4].ImplementationType.ShouldBe(typeof(SampleContextStepM)),
            () => result[5].ImplementationType.ShouldBe(typeof(SampleContextStepZ))
        );
    }

    [Fact]
    public void GetDescriptorsFor_ReturnsIEnumerable_ForValidCustomStepInterface_WithMultipleFilters()
    {
        var result = _assemblies.GetDescriptorsFor<ISampleContextStep>([TestConstants.Filter2, TestConstants.Filter1]).ToList();
        result.ShouldNotBeEmpty();

        result.ShouldSatisfyAllConditions
        (
            () => result.Count().ShouldBe(7),
            () => result[0].ImplementationType.ShouldBe(typeof(SampleContextStepA)),
            () => result[1].ImplementationType.ShouldBe(typeof(SampleContextStepC)),
            () => result[2].ImplementationType.ShouldBe(typeof(SampleContextStepB)),
            () => result[3].ImplementationType.ShouldBe(typeof(SampleContextStepF1)),
            () => result[4].ImplementationType.ShouldBe(typeof(SampleContextStepF2)),
            () => result[5].ImplementationType.ShouldBe(typeof(SampleContextStepM)),
            () => result[6].ImplementationType.ShouldBe(typeof(SampleContextStepZ))
        );
    }

    [Fact]
    public void SafeGetTypes_ReturnsTypes_WhenDelegateSucceeds()
    {
        var expected = new[] { typeof(string), typeof(int) };
        var result = PipeForge.Extensions.AssemblyExtensions.SafeGetTypes(() => expected);
        result.ShouldBe(expected);
    }

    [Fact]
    public void SafeGetTypes_ReturnsFilteredTypes_WhenReflectionTypeLoadExceptionThrown()
    {
        var types = new Type?[] { typeof(string), null, typeof(int) };
        var ex = new ReflectionTypeLoadException(types, []);
        var result = PipeForge.Extensions.AssemblyExtensions.SafeGetTypes(() => throw ex);
        result.ShouldBe([typeof(string), typeof(int)]);
    }

    [Fact]
    public void SafeGetTypes_ReturnsEmpty_WhenUnexpectedExceptionThrown()
    {
        var result = PipeForge.Extensions.AssemblyExtensions.SafeGetTypes(() => throw new InvalidOperationException());
        result.ShouldBeEmpty();
    }
}
