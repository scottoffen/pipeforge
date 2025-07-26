---
sidebar_position: 2
title: Pipeline Steps
---

Pipeline steps are the building blocks of a PipeForge pipeline. Each step performs a discrete unit of work and passes control to the next step in the sequence. Steps operate on a shared **context** object - a strongly typed class that carries data and state throughout the pipeline.

PipeForge treats each step as an independent, discoverable component. You write them as plain classes, define their order and optional filters via attributes, and rely on constructor injection to bring in any needed services.

Each step must implement `IPipelineStep<TContext>`, which consists of one method and four metadata properties:

| Member                       | Type     | Description                                                                                    |
| --------------------------- | -------- | ----------------------------------------------------------------------------------------------- |
| `Name`                      | `string` | A human-readable name for the step. Useful for logging, diagnostics, and documentation.         |
| `Description`               | `string` | A brief description of what the step does. Useful for tooling and developer insight.            |
| `MayShortCircuit`           | `bool`   | Indicates whether this step may intentionally stop pipeline execution early.                    |
| `ShortCircuitCondition`     | `string` | Describes the condition under which this step might short-circuit. Used for documentation only. |
| `InvokeAsync(context, next)`| `Task`   | Executes the step's logic. To continue the pipeline, call the `next` delegate.                  |

The optional metadata properties do not affect execution - they exist solely to aid in diagnostics and documentation.

:::tip[Tip]
Pipeline steps are just regular classes. You can inject dependencies into their constructors - making them fully compatible with your existing services, business logic, and data access layers.
:::

## Extending `PipelineStep<T>`

While you can implement `IPipelineStep<TContext>` directly, the recommended approach is to derive from the `PipelineStep<TContext>` base class. This abstract class provides default implementations for the optional metadata properties (`Name`, `Description`, etc.), allowing you to override only what you need.

```csharp title="PipeForge.PipelineStep.cs"
public abstract class PipelineStep<TContext> : IPipelineStep<TContext>
    where TContext : class
{
    public virtual string? Description { get; set; } = null;

    public virtual bool MayShortCircuit { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual string? ShortCircuitCondition { get; set; } = null;

    public abstract Task InvokeAsync(TContext context, PipelineDelegate<TContext> next, CancellationToken cancellationToken = default);
}
```

When extending this class, you can optionally assign metadata values in the constructor or directly in the property initializers.

```csharp title="AddToContextStep.cs"
public class AddToContextStep : PipelineStep<SampleContext>
{
    public AddToContextStep()
    {
        Name = "AddToContextStep";
    }

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
```

Using a base class keeps your steps clean and focused, makes metadata easy to inspect at runtime, and improves testability. If you prefer, you can always define your own base class.

## Short Circuiting Execution

Pipeline steps can halt execution early by choosing *not* to call the `next()` delegate. This is known as **short-circuiting** - it prevents remaining steps from being instantiated or executed.

Short-circuiting is useful when:
- A failure or validation check occurs and further processing should stop.
- A decision point is reached where later steps are unnecessary.
- Performance optimizations are needed for conditional flows.

The `MayShortCircuit` property can be set to indicate that a step *may* short-circuit, and `ShortCircuitCondition` can be used to describe when and why - these are for documentation only.

```csharp title="ShortCircuitStep.cs"
public class ShortCircuitStep : PipelineStep<SampleContext>
{
    public ShortCircuitStep()
    {
        Name = "ShortCircuitStep";
        Description = "Stops execution if fewer than two steps have run.";
        MayShortCircuit = true;
        ShortCircuitCondition = "StepCount < 2";
    }

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);

        // Stop execution early if not enough steps have run
        if (context.StepCount < 2) return;

        await next(context, cancellationToken);
    }
}
```

In this example, the pipeline stops early unless at least two steps have been recorded in the context. Because steps are lazily instantiated, any remaining steps in the pipeline will not even be created.

## Adding the `[PipelineStep]` Attribute

To be automatically discovered, each pipeline step must be decorated with the `[PipelineStep]` attribute. This attribute defines the order in which the step should execute.

Steps with lower order values run earlier in the pipeline. If two steps share the same order, their relative execution is undefined unless explicitly ordered another way.

```csharp title="Step1.cs"
[PipelineStep(1)]
public class Step1 : PipelineStep<SampleContext>
{
    public Step1()
    {
        Name = "Step1";
        Description = "This is the first step in the pipeline.";
    }

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
```

:::tip[Tip]

- Steps that use different context types can reuse the same order value - ordering only applies within the same pipeline.
- Steps using the same context type and the same order value will be executed in arbitrary order.

:::

### Optional Step Filters

You can limit when a pipeline step is registered by adding one or more filter values to the [PipelineStep] attribute. Filters allow you to exclude steps from registration unless a matching filter is explicitly provided during discovery.

This is useful for environment-based behavior (e.g. only include dev/test steps), tenant-specific logic, or feature flags.

```csharp title="Step2.cs"
[PipelineStep(2, "Development", "Testing")]
public class Step2 : PipelineStep<SampleContext>
{
    public Step2()
    {
        Name = "Step2";
        Description = "This step only runs in development or testing filters.";
    }

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
```

:::danger[Caution]

- Steps **without** a `Filter` parameter will always be registered during the step discovery and registration process. 
- Steps **with** `Filter` parameter will only be registered if a matching filter is passed to the [registration extension method](./pipeline-registration.md).

:::


## Creating Custom Step Interfaces

When building multiple pipelines (especially for the same context), it can be helpful to define a custom step interface for each one. This allows you to isolate steps to a specific pipeline, organize your code more clearly, and register and resolve only the relevant steps using dependency injection.

To do this, you create a new interface that inherits from `IPipelineStep<TContext>` and use it as the marker type for step registration and execution.

```csharp title="ISampleContextStep.cs"
// Custom interface for steps in the SampleContext pipeline
public interface ISampleContextStep : IPipelineStep<SampleContext> { }
```

Then, have your step class implement the custom interface:

```csharp title="SampleContextStepA"
[PipelineStep(1)]
public class SampleContextStepA : PipelineStep<SampleContext>, ISampleContextStep
{
    public SampleContextStepA()
    {
        Name = "A";
        Description = "A step in the SampleContext pipeline.";
    }

    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
```

:::tip[I'm still extending `PipelineStep<T>`]

Extending `PipelineStep<T>` is optional, but helpful - it lets you avoid implementing the entire interface manually. Pipeline discovery and registration will still work even if you inherit from a different base class or none at all.

:::

When registering or resolving the pipeline, we'll use the custom interface as the step type. This pattern helps:
- Prevent cross-contamination between unrelated pipelines
- Simplify discovery and debugging
- Maintain clear architectural boundaries

You can reuse the same `SampleContext` type across different pipelines if needed - just define separate step interfaces to control which steps apply to each one, and then use the appropriate [registration extension method](./pipeline-registration.md).
