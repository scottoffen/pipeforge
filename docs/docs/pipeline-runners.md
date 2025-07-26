---
sidebar_position: 3
title: Pipeline Runners
---

A pipeline runner is responsible for executing a pipeline by resolving and invoking each registered step in order. It handles lazy instantiation, short-circuiting, cancellation, and exceptions during execution.

## Runner Interfaces and Defaults

All runners implement the `IPipelineRunner<TContext, TStepInterface>` interface, where `TStepInterface` must implement `IPipelineStep<TContext>`. In most cases, `TStepInterface` is a custom interface used to identify which steps belong to a specific pipeline.

If you're not using a custom step interface, you can use the convenience interface `IPipelineRunner<TContext>`. This is just a shorthand for `IPipelineRunner<TContext, IPipelineStep<TContext>>`.

```csharp title="IPipelineRunner.cs"
IPipelineRunner<TContext, TStepInterface>
    where T : class
    where TStepInterface : IPipelineStep<TContext>
{ }

IPipelineRunner<TContext> : IPipelineRunner<TContext, IPipelineStep<TContext>>
    where TContext : class
{ }
```

PipeForge provides default runner implementations for `IPipelineRunner<TContext, TStepInterface>` and `IPipelineRunner<TContext>`, so you don't need to write a custom runner unless you want to customize behavior or manage dependency resolution differently.

If you're using custom step interfaces (e.g. to support multiple pipelines), it can be helpful to define a matching custom runner. PipeForge provides a base class, `PipelineRunner<TContext, TStepInterface>`, to make this easy:

```csharp
public interface ISampleContextRunner
    : IPipelineRunner<SampleContext, ISampleContextStep> { }

public class SampleContextRunner
    : PipelineRunner<SampleContext, ISampleContextStep>, ISampleContextRunner { }
```

This approach allows you to resolve your runner via `ISampleContextRunner`, rather than repeating the full generic signature. When using PipeForge's registration extensions, your custom runner will be registered automatically - no manual wiring required.

## Custom Runners

If you need to extend the default behavior (e.g. to add logging, diagnostics, or custom instrumentation), you can create your own runner by implementing `IPipelineRunner<TContext, TStepInterface>` directly, and optionally extending `PipelineRunner<TContext, TStepInterface>`.