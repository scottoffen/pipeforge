---
sidebar_position: 4
title: Running Pipelines
---

# Running Pipelines

Once your pipeline steps are discovered and registered, you can run the pipeline by resolving and invoking an `IPipelineRunner<T>` from the dependency injection container.

Each `IPipelineRunner<T>` executes the pipeline steps for a specific context type `T`, in the order defined by their `[PipelineStep]` attribute.

## Example Usage

```csharp title="Executing a Pipeline"
[PipelineStep(1)]
public class StepA : PipelineStep<StepContext>
{
    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("A1");
        await next(context, cancellationToken);
    }
}

[PipelineStep(2)]
public class StepB : PipelineStep<StepContext>
{
    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("B2");
        await next(context, cancellationToken);
    }
}

[PipelineStep(3)]
public class StepC : PipelineStep<StepContext>
{
    public override async Task InvokeAsync(StepContext context, PipelineDelegate<StepContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("C3");
        await next(context, cancellationToken);
    }
}

var context = new SampleContext();
var services = new ServiceCollection();
services.AddPipelineFor<SampleContext>();

var provider = services.BuildServiceProvider();
var runner = provider.GetRequiredService<IPipelineRunner<SampleContext>>();

await runner.ExecuteAsync(context);

Console.WriteLine(context.ToString()); // e.g. "A1,B2,C3"
```

Each registered pipeline step for `SampleContext` will be executed in order, unless short-circuited by a step that chooses not to call `next()`.

## Behavior Details

- The pipeline is lazily instantiated. Only the steps needed for the current run will be resolved from the container.
- If a step short-circuits (by not calling `next()`), remaining steps will **not** be resolved or executed.
- Exceptions thrown in any step will bubble up unless explicitly handled inside the step.
- Unhandled exceptions during pipeline execution will be wrapped in a `PipelineExecutionException<T>`, which preserves the original exception as an inner exception.

## Stateless Runners

Pipeline runners are stateless and safe to reuse. You can execute the same runner multiple times with different contexts:

```csharp
var first = new SampleContext();
await runner.ExecuteAsync(first);

var second = new SampleContext();
await runner.ExecuteAsync(second);
```

## Conclusion

Your pipeline is now fully operational. In the next section, you'll learn how to test pipelines and steps independently.
