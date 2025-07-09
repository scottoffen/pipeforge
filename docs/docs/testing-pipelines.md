---
sidebar_position: 5
title: Testing Pipelines
---

# Testing Pipelines

One of PipeForge's core design goals is testability. Because steps are resolved lazily and operate on a shared context, individual steps and full pipelines can be easily tested using standard unit testing practices.

## Testing Individual Steps

Pipeline steps can be tested like any other class. Just instantiate the step and call `InvokeAsync()` with a test context and a stub `next` delegate.

```csharp title="AddStepTest.cs"
public class AddStepTest
{
    private class TestStep : PipelineStep<SampleContext>
    {
        public TestStep() => Name = "TestStep";

        public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
        {
            context.AddStep(Name);
            return next(context, cancellationToken);
        }
    }

    [Fact]
    public async Task StepAddsNameToContext()
    {
        var context = new SampleContext();
        var step = new TestStep();

        await step.InvokeAsync(context, (_, _) => Task.CompletedTask);

        Assert.Equal("TestStep", context.ToString());
    }
}
```

## Testing Short-Circuit Behavior

You can verify whether a step properly short-circuits by using a stub `next` delegate that fails the test if called.

```csharp title="ShortCircuitTest.cs"
public class ShortCircuitTest
{
    private class ShortCircuitStep : PipelineStep<SampleContext>
    {
        public ShortCircuitStep() => Name = "SkipRest";

        public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
        {
            context.AddStep(Name);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task StepShortCircuitsPipeline()
    {
        var context = new SampleContext();
        var step = new ShortCircuitStep();

        await step.InvokeAsync(context, (_, _) => throw new Exception("Should not be called"));

        Assert.Equal("SkipRest", context.ToString());
    }
}
```

## Testing the Full Pipeline

You can test the full pipeline by registering steps with a service provider and using `IPipelineRunner<T>`. This is especially useful for verifying composition, ordering, and side effects.

```csharp title="PipelineIntegrationTest.cs"
public class PipelineIntegrationTest
{
    private class StepA : PipelineStep<SampleContext>
    {
        public StepA() => Name = "A";

        public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
        {
            context.AddStep(Name);
            return next(context, cancellationToken);
        }
    }

    private class StepB : PipelineStep<SampleContext>
    {
        public StepB() => Name = "B";

        public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
        {
            context.AddStep(Name);
            return next(context, cancellationToken);
        }
    }

    [Fact]
    public async Task PipelineExecutesStepsInOrder()
    {
        var services = new ServiceCollection();
        services.AddPipelineStep<StepA>();
        services.AddPipelineStep<StepB>();
        services.AddTransient<IPipelineRunner<SampleContext>, PipelineRunner<SampleContext>>();

        var provider = services.BuildServiceProvider();
        var runner = provider.GetRequiredService<IPipelineRunner<SampleContext>>();

        var context = new SampleContext();
        await runner.ExecuteAsync(context);

        Assert.Equal("A,B", context.ToString());
    }
}
```

## Summary

* Steps are just regular classes and can be tested independently.
* You control the `next` delegate to test full, partial, or short-circuited runs.
* The DI container can be configured in tests to simulate realistic pipeline composition.
