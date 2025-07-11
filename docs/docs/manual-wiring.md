---
sidebar_position: 7
title: Manual Wiring
---

# Using PipeForge Without Dependency Injection

PipeForge is designed to integrate smoothly with dependency injection, but it's flexible enough to be used without it. Manual wiring may be useful in testing scenarios, lightweight environments, or when you want full control over pipeline composition.

The primary disadvantage of this approach is that you will need to be responsible for instantiating all of the dependencies of each step.

## Manually Constructing a Pipeline

Manually building a pipeline is easily done using the `PipelineBuilder<T>` instance returned from the static method `Pipeline.CreateFor<T>()`, which can take an optional instance of `ILoggerFactory` for use in the execution of the resulting `PipelineRunner<T>`.

Add steps to the pipeline in the order you want them to execute using the chainable methods:
- `Add<TStep>()` (for steps with a parameterless constructor)
- `AddStep<TStep>(Func<TStep> func)` (for steps that have constructor parameters)

```csharp title="Steps Used"
private abstract class TestStep : PipelineStep<SampleContext>
{
    public override async Task InvokeAsync(
        SampleContext context,
        PipelineDelegate<SampleContext> next,
        CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}

private class StepA : TestStep
{
    public StepA() => Name = "A";
}

private class StepB : TestStep
{
    public StepB() => Name = "B";
}

private class StepC : TestStep
{
    public StepC() => Name = "C";
}

private class StepD : TestStep
{
    public StepD(string name) => Name = name;
}
```

```csharp title="Manual Pipeline Setup"
var stepName = "Hello";
var pipeline = Pipeline.CreateFor<SampleContext>()
    .WithStep<StepA>()
    .WithStep<StepB>()
    .WithStep<StepC>()
    .WithStep(() => new StepD(stepName))
    .Build();

var context = new SampleContext();
await pipeline.ExecuteAsync(context);

Console.WriteLine(context);
// Should be:
// A,B,C,Hello
```

## Conclusion

Manual wiring allows you to build and run a pipeline without relying on a DI container. While this approach is less common for production use, it provides maximum control for testing and minimal-host scenarios.