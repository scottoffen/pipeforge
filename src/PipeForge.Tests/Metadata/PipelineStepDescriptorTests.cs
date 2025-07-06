using PipeForge.Metadata;
using PipeForge.Tests.TestUtils;

namespace PipeForge.Tests.Metadata;

public class PipelineStepDescriptorTests
{
    [PipelineStep(42, isEnabled: false, environment: "QA")]
    private class AnnotatedStep : IPipelineStep<TestContext>
    {
        public string Name => "Test";
        public string Description => "";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;
        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private class UnannotatedStep : IPipelineStep<TestContext>
    {
        public string Name => "NoAttr";
        public string Description => "";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;
        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [Fact]
    public void Constructor_Extracts_Metadata_From_Attribute()
    {
        var descriptor = new PipelineStepDescriptor(typeof(AnnotatedStep));

        descriptor.ImplementationType.ShouldBe(typeof(AnnotatedStep));
        descriptor.Order.ShouldBe(42);
        descriptor.IsEnabled.ShouldBeFalse();
        descriptor.Environment.ShouldBe("QA");
    }

    [Fact]
    public void Constructor_Throws_If_Type_Missing_Attribute()
    {
        var ex = Should.Throw<InvalidOperationException>(() =>
        {
            _ = new PipelineStepDescriptor(typeof(UnannotatedStep));
        });

        ex.Message.ShouldContain(nameof(UnannotatedStep));
    }
}
