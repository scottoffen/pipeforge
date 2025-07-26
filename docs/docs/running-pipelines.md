---
sidebar_position: 5
title: Running Pipelines
---

Each runner executes the steps associated with a specific context type, in the order they were registered. If you registered steps individually, execution order follows registration order. If you used pipeline discovery, step order is determined by the `[PipelineStep]` attribute.

Once your pipeline has been registered, you can execute it by resolving a suitable `IPipelineRunner` from the dependency injection container. The type of runner you resolve depends on how the pipeline was registered:

| Registration Method                       | Runner to Resolve                           |
| ----------------------------------------- | ------------------------------------------- |
| Registered with default step interface    | `IPipelineRunner<TContext>`                 |
| Registered with custom step interface     | `IPipelineRunner<TContext, TStepInterface>` |
| Registered with a custom runner interface | e.g. `ISampleContextRunner`                 |

## Example Usage

```csharp title="Executing a Pipeline"
[PipelineStep(1)]
public class StepA : PipelineStep<SampleContext>
{
    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("A1");
        await next(context, cancellationToken);
    }
}

[PipelineStep(2)]
public class StepB : PipelineStep<SampleContext>
{
    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("B2");
        await next(context, cancellationToken);
    }
}

[PipelineStep(3)]
public class StepC : PipelineStep<SampleContext>
{
    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("C3");
        await next(context, cancellationToken);
    }
}

var services = new ServiceCollection();
services.AddPipeline<SampleContext>();

var provider = services.BuildServiceProvider();
var runner = provider.GetRequiredService<IPipelineRunner<SampleContext>>();

var context = new SampleContext();
await runner.ExecuteAsync(context);

Console.WriteLine(context.ToString()); // e.g. "A1,B2,C3"
```

Each registered pipeline step for `SampleContext` will be executed in order, unless short-circuited by a step that chooses not to call `next()`.

## Behavior Details

- The pipeline is lazily instantiated. Only the steps needed for the current run will be resolved from the container.
- If a step short-circuits (by not calling `next()`), remaining steps will **not** be resolved or executed.
- Exceptions thrown in any step will bubble up unless explicitly handled inside the step.
- Unhandled exceptions during pipeline execution will be wrapped in a `PipelineExecutionException<TContext>`, which preserves the original exception as an inner exception.

## Stateless Runners

Pipeline runners are stateless and safe to reuse. Steps are resolved fresh from the container for each run, to honor service lifetime. You can execute the same runner multiple times with different contexts:

```csharp
var first = new SampleContext();
await runner.ExecuteAsync(first);

var second = new SampleContext();
await runner.ExecuteAsync(second);
```
