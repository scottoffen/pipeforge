---
sidebar_position: 6
title: Manual Composition
---

PipeForge is designed to integrate seamlessly with dependency injection, but it's flexible enough to support manual composition. This is useful in testing scenarios, minimal environments, or when you need complete control over how the pipeline is configured and executed.

One tradeoff with this approach is that it bypasses automatic step discovery and attribute-based filtering. As a result, you'll need to manage step order and registration logic explicitly, which may lead to inconsistencies if your production pipeline uses assembly scanning.

## Creating a Pipeline

Manual composition is handled through the fluent API exposed by `PipelineBuilder<TContext>`, which you can create via:

```csharp
var builder = Pipeline.CreateFor<SampleContext>();
```

Steps are added in the desired execution order using the following chainable methods:

| Method                                                                          | Description                                                 |
| ------------------------------------------------------------------------------- | ----------------------------------------------------------- |
| `WithStep<TStep>()`                                                             | Registers a class that implements `IPipelineStep<TContext>` |
| `WithStep(Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task>)` | Registers an inline delegate step                           |

If your steps have dependencies, you can register them using `ConfigureServices(Action<IServiceCollection> configure)`. This allows you to take full advantage of constructor injection even in a manually composed pipeline, using a lightweight internal service container managed by the builder.

### Example

The following example demonstrates how to manually build and execute a pipeline using a mix of class-based and inline delegate steps. It also shows how to register services required by steps through the builder's internal service container. Class definitions for the steps and services appear below.

```csharp
var builder = Pipeline.CreateFor<SampleContext>();

builder.ConfigureServices(services =>
{
    services.AddSingleton<IMyDependency, MyDependency>();
});

builder.WithStep<StepA>()
       .WithStep<StepB>()
       .WithStep((context, next, cancellationToken) =>
       {
           context.AddStep("InlineStep");
           return next(context, cancellationToken);
       });

var runner = builder.Build();

var context = new SampleContext();
await runner.ExecuteAsync(context);

Console.WriteLine(context); // Outputs step history


public interface IMyDependency
{
    void DoSomething();
}

public class MyDependency : IMyDependency
{
    public void DoSomething() => Console.WriteLine("Dependency invoked");
}

public class StepA : PipelineStep<SampleContext>
{
    private readonly IMyDependency _dependency;

    public StepA(IMyDependency dependency)
    {
        _dependency = dependency;
        Name = "StepA";
    }

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        _dependency.DoSomething();
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}

public class StepB : PipelineStep<SampleContext>
{
    public StepB()
    {
        Name = "StepB";
    }

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
```

This example shows how to compose a simple pipeline with both concrete and delegate-based steps, and then execute it manually.



## Conclusion

Manual composition allows you to build and run a pipeline without relying on a DI container. While this approach is less common for production use, it provides maximum control for configuration, **testing** and minimal-host scenarios.