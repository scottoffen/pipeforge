using Microsoft.Extensions.DependencyInjection;
using PipeForge.Metadata;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests.PipelineRegistration;

public class RegisterStepTests
{
    [Fact]
    public void RegisterStep_RegistersInterfaceAndLazyType()
    {
        var services = new ServiceCollection();
        var descriptor = StepTypeDescriptor.Create<ISampleContextStep>(typeof(SampleContextStepA));

        services.RegisterStep(descriptor, ServiceLifetime.Transient, null);

        var provider = services.BuildServiceProvider();
        var step = provider.GetRequiredService<SampleContextStepA>();
        var intr = provider.GetRequiredService<ISampleContextStep>();
        var lazy = provider.GetRequiredService<Lazy<ISampleContextStep>>();
    }

    [Fact]
    public void RegisterStep_Throws_WhenStepIsAlreadyRegistered()
    {
        var descriptor = StepTypeDescriptor.Create<ISampleContextStep>(typeof(SampleContextStepA));

        var services = new ServiceCollection();
        services.AddTransient<ISampleContextStep, SampleContextStepA>();
#if DEBUG
        var ex = Should.Throw<InvalidOperationException>(() =>
        {
            services.RegisterStep(descriptor, ServiceLifetime.Transient, null);
        });

        ex.Message.ShouldBe(string.Format(PipeForge.PipelineRegistration.MessageStepAlreadyRegistered, descriptor.TypeName));
#endif
    }
}
