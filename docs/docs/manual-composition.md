---
sidebar_position: 7
title: Manual Composition
---

# Manual Composition

PipeForge is designed to integrate seamlessly with dependency injection, but it's flexible enough to support manual composition. This can be useful in testing scenarios, minimal environments, or when you need full control over pipeline configuration.

The main drawbacks of this approach are that you'll be responsible for manually instantiating all dependencies for each step, and the resulting pipeline may not accurately reflect the behavior of your production configuration.

## Creating a Pipeline

Manual pipeline composition is straightforward using the fluent API exposed by `PipelineBuilder<T>`, which is returned from the static method `Pipeline.CreateFor<T>()`. You can optionally pass an `ILoggerFactory` to enable logging during execution by the resulting `PipelineRunner<T>`.

Add steps to the pipeline in the desired execution order using the fluent, chainable methods:

- `Add<TStep>()` - for steps with a parameterless constructor
- `AddStep<TStep>(Func<TStep> factory)` - for steps that require constructor parameters

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

Note that `StepD` lacks a parameterless constructor, which means it can't be added using `Add<TStep>()`. Instead, you'll need to use `AddStep<TStep>(Func<TStep>)` to manually supply its dependencies - for example, if the step requires a configuration value, a logger, or a service instance.

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

## Advanced Scenarios

In more advanced scenarios, the factory delegate can create a scope and resolve dependencies from the scoped service provider before constructing the step. This is especially useful when the step relies on services with scoped lifetimes, such as per-request context or transient infrastructure components. For example:

```
builder.WithStep(() =>
{
    using var scope = serviceProvider.CreateScope();
    var scopedProvider = scope.ServiceProvider;
    var name = scopedProvider.GetRequiredService<IOptions<MySettings>>().Value.Name;
    return new StepD(name);
});
```

## Conclusion

Manual composition allows you to build and run a pipeline without relying on a DI container. While this approach is less common for production use, it provides maximum control for configuration, **testing** and minimal-host scenarios.