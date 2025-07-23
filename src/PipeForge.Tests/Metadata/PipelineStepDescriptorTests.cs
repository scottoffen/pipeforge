using PipeForge.Metadata;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.Metadata;

public class PipelineStepDescriptorTests
{
    private class HasNoAttributeStep : SampleContextStep
    { }

    [PipelineStep(1)]
    private class HasNoFilterStep : SampleContextStep
    {
    }

    [PipelineStep(2, TestConstants.Filter1)]
    private class HasOneFilterStep : SampleContextStep
    {
    }

    [PipelineStep(3, TestConstants.Filter1, TestConstants.Filter2)]
    private class HasTwoFiltersStep : SampleContextStep
    {
    }

    [Fact]
    public void Constructor_SetsProperties_WhenAttributeIsPresent()
    {
        var descriptor = new PipelineStepDescriptor(typeof(HasNoFilterStep));

        descriptor.ImplementationType.ShouldBe(typeof(HasNoFilterStep));
        descriptor.Order.ShouldBe(1);
        descriptor.Filters.ShouldBeEmpty();
    }

    [Fact]
    public void Constructor_SetsProperties_WhenAttributeHasOneFilter()
    {
        var descriptor = new PipelineStepDescriptor(typeof(HasOneFilterStep));

        descriptor.ImplementationType.ShouldBe(typeof(HasOneFilterStep));
        descriptor.Order.ShouldBe(2);
        descriptor.Filters.ShouldNotBeEmpty();
        descriptor.Filters.Count().ShouldBe(1);
        descriptor.Filters.ShouldContain(TestConstants.Filter1);
    }

    [Fact]
    public void Constructor_SetsProperties_WhenAttributeHasMultipleFilters()
    {
        var descriptor = new PipelineStepDescriptor(typeof(HasTwoFiltersStep));

        descriptor.ImplementationType.ShouldBe(typeof(HasTwoFiltersStep));
        descriptor.Order.ShouldBe(3);
        descriptor.Filters.ShouldNotBeEmpty();
        descriptor.Filters.Count().ShouldBe(2);
        descriptor.Filters.ShouldContain(TestConstants.Filter1);
        descriptor.Filters.ShouldContain(TestConstants.Filter2);
    }

    [Fact]
    public void Constructor_ThrowsInvalidOperationException_WhenAttributeIsMissing()
    {
        var ex = Should.Throw<InvalidOperationException>(() =>
        {
            _ = new PipelineStepDescriptor(typeof(HasNoAttributeStep));
        });

        ex.Message.ShouldContain(nameof(PipelineStepAttribute));
        ex.Message.ShouldContain(nameof(HasNoAttributeStep));
    }
}
