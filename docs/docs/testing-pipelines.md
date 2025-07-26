---
sidebar_position: 7
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

You can test the full pipeline using [`PipelineBuilder<TContext>`](./manual-composition.md).

## Summary

* Steps are just regular classes and can be tested independently.
* You control the `next` delegate to test full, partial, or short-circuited runs.
* Use `PipelineBuilder<TContext>` simulate realistic pipeline composition.
