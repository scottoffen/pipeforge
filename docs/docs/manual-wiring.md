---
sidebar_position: 7
title: Manual Wiring
---

# Using PipeForge Without Dependency Injection

PipeForge is designed to integrate smoothly with dependency injection, but it's flexible enough to be used without it. Manual wiring may be useful in testing scenarios, lightweight environments, or when you want full control over pipeline composition.

## Manually Constructing a Pipeline

You can manually create a list of steps and pass them to a `PipelineRunner<T>`. Each step should be wrapped in a `Lazy<IPipelineStep<T>>` to maintain compatibility with PipeForge's lazy instantiation pattern.

```csharp title="Manual Pipeline Setup"
public class StepA : PipelineStep<SampleContext>
{
    public StepA() => Name = "A";

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}

public class StepB : PipelineStep<SampleContext>
{
    public StepB() => Name = "B";

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}

public class StepC : PipelineStep<SampleContext>
{
    public StepShortCircuit() => Name = "C";

    public override Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}

var steps = new List<Lazy<IPipelineStep<SampleContext>>>
{
    new(() => new StepA()),
    new(() => new StepC()),
    new(() => new StepB())
};

var runner = new PipelineRunner<SampleContext>(steps);

var context = new SampleContext();
await runner.ExecuteAsync(context);

Console.WriteLine($"Final context: {context}");
```

## Conclusion

Manual wiring allows you to build and run a pipeline without relying on a DI container. While this approach is less common for production use, it provides maximum control for testing and minimal-host scenarios.