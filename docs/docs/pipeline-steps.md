---
sidebar_position: 2
title: Pipeline Steps
---

# Pipeline Steps

Each pipeline step operates on a strongly typed context, specified via a generic parameter.

A pipeline step consists of a single method and four optional metadata properties.

| Property / Method            | Type     | Description                                                                              |
| ---------------------------- | -------- | ---------------------------------------------------------------------------------------- |
| `Name`                       | `string` | A human-readable name for the step. Useful for logging, diagnostics, and documentation.  |
| `Description`                | `string` | A brief description of what the step does. Primarily used for documentation and tooling. |
| `MayShortCircuit`            | `bool`   | Indicates whether this step may choose to stop pipeline execution early.                 |
| `ShortCircuitCondition`      | `string` | Describes the condition under which this step may short-circuit. For documentation only. |
| `InvokeAsync(context, next)` | `Task`   | Executes the step's logic. To continue execution, call the `next` delegate.              |

The optional metadata properties do not impact step registration or execution, but can be used to provide developer documentation about the step, and aide in troubleshooting.

:::tip[Tip]

Because pipeline steps are plain classes, you can inject dependencies through their constructors - enabling full integration with your existing services, business logic, and data access layers.

:::


## Extending `PipelineStep<T>`

Using the `PipelineStep<T>` abstract class that provides default implementations for the optional metadata properties of `IPipelineStep<T>` is the recommended approach to creating pipeline steps. You can optionally set the metadata properties in the constructor.

```csharp title="AddToContextStep.cs"
public class AddToContextStep : PipelineStep<StepContext>
{
    public AddToContextStep()
    {
        // Sets the Name property
        Name = "AddToContextStep";
    }

    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);
        await next(context, cancellationToken);
    }
}
```

## Short Circuiting Execution

Pipeline steps can short-circuit the pipeline execution by not calling the `next()` delegate that is passed to the method.

```csharp title="ShortCircuitStep.cs"
public class ShortCircuitStep : PipelineStep<StepContext>
{
    public ShortCircuitStep()
    {
        // Sets the Name property
        Name = "ShortCircuitStep";
    }

    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep(Name);

        // If context.Steps is less than 2, the pipeline won't continue execution.
        if (context.Steps >= 2)
            await next(context, cancellationToken);
    }
}
```

## Adding the `[PipelineStep]` Attribute

To be discoverable, each pipeline step must be decorated with the [PipelineStep] attribute. This attribute defines the order in which pipeline steps should execute.

```csharp title="Step1.cs"
[PipelineStep(1)]
public class Step1 : PipelineStep<SampleContext>
{
    // This is the first step that will execute in the pipeline
}
```

:::tip[Tip]

- Multiple steps that are not using the same context will not be impacted by using the same order value.
- Multiple steps that are using the same context and have the same order value will be ordered arbitrarily.

:::

### Environment-Specific Steps

You can limit which environments a step will be registered in by adding the `Environment` parameter to the `PipelineStep` attribute. This is useful to add steps that will only run in Development environments.

```csharp title="Step2.cs"
[PipelineStep(2, "Development")]
public class Step2 : PipelineStep<SampleContext>
{
    // This is the second step that will execute in the pipeline
}
```

:::tip[Tip]

Despite being named "Environment", this value is not limited solely to that purpose. You can use it filter out steps using any kind of value or feature flag that is known prior to step discovery and registration.

:::

:::danger[Caution]

- Steps that do NOT have an `Environment` parameter in the attribute will always be registered during the step discovery process. 
- Steps that DO have an `Environment` parameter in the attribute will only be registered if the same value is passed to the [registration extension method](./step-discovery.md).

:::

## Conclusion

Now that you've defined your pipeline steps, the next step is to discover and register them.