using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PipeForge.Tests.Steps;

namespace PipeForge.Tests;

public class PipelineBuilderTests
{
    // This sample interface and implementation are used to validate
    // that services can be added and resolved using the pipeline builder.
    private interface IRandomNumberService
    {
        int GetRandomNumber();
    }

    private class RandomNumberService : IRandomNumberService
    {
        private static readonly Random _random = new();

        public int GetRandomNumber()
        {
            return _random.Next();
        }
    }

    private class BuilderStep1 : PipelineStep<SampleContext>
    {
        public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
        {
            context.AddStep("BuilderStep1");
            return next(context, cancellationToken);
        }
    }

    private class BuilderStep2 : PipelineStep<SampleContext>
    {
        private readonly IRandomNumberService _randomNumberService;

        public BuilderStep2(IRandomNumberService randomNumberService)
        {
            _randomNumberService = randomNumberService;
        }

        public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
        {
            context.AddStep("BuilderStep2");
            await next(context, cancellationToken);
            context.AddStep(_randomNumberService.GetRandomNumber().ToString());
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PipelineBuilder_CreatesAndRunsPipeline(bool provideLogger)
    {
        ILoggerFactory? loggerFactory = provideLogger ? new LoggerFactory() : null;

        var context = new SampleContext();
        var builder = Pipeline.CreateFor<SampleContext>(loggerFactory);

        // Test adding a service to the pipeline
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IRandomNumberService, RandomNumberService>();
        });

        builder.WithStep<SampleContextStepA>();

        // Test adding a step with a delegate
        builder.WithStep((ctx, next, cancellationToken) =>
        {
            ctx.AddStep("BuilderStep0");
            return next(ctx, cancellationToken);
        });

        // Test adding a step using a class
        builder.WithStep<BuilderStep1>();

        // Test adding a step using a class with dependencies
        builder.WithStep<BuilderStep2>();

        // Test adding a step with a delegate
        builder.WithStep((ctx, next, cancellationToken) =>
        {
            ctx.AddStep("BuilderStep3");
            return next(ctx, cancellationToken);
        });

        var pipeline = builder.Build();

        await pipeline.ExecuteAsync(context);

        context.Steps.Count.ShouldBe(6);
        context.Steps[0].ShouldBe(SampleContextStepA.StepName);
        context.Steps[1].ShouldBe("BuilderStep0");
        context.Steps[2].ShouldBe("BuilderStep1");
        context.Steps[3].ShouldBe("BuilderStep2");
        context.Steps[4].ShouldBe("BuilderStep3");
        int.TryParse(context.Steps[5], out _).ShouldBeTrue();
    }

    [Fact]
    public void PipelineBuilder_ThrowsException_WhenTypeImplementsIPipelineStep()
    {
        Should.Throw<ArgumentException>(() =>
        {
            _ = Pipeline.CreateFor<SampleContextStepA>();
        });
    }
}
