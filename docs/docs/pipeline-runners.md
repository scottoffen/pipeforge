---
sidebar_position: 3
title: Pipeline Runners
---

A pipeline runner is responsible for executing a pipeline by resolving and invoking each registered step in order. It handles lazy instantiation, short-circuiting, cancellation, and exceptions during execution.

## Runner Interfaces and Defaults

All runners implement the `IPipelineRunner<TContext, TStepInterface>` interface. The `TStepInterface` identifies which steps belong to the pipeline, and should typically be a custom interface that extends `IPipelineStep<TContext>`.

If you are not using a custom step interface, you can use the convenience interface `IPipelineRunner<TContext>`, which is a shorthand for the generic form using `IPipelineStep<TContext>` as the step interface.

```csharp title="IPipelineRunner.cs"
IPipelineRunner<TContext, TStepInterface>
    where T : class
    where TStepInterface : IPipelineStep<TContext>
{ }

IPipelineRunner<TContext> : IPipelineRunner<TContext, IPipelineStep<TContext>>
    where TContext : class
{ }
```

PipeForge provides default runner implementations for `IPipelineRunner<TContext, TStepInterface>` and `IPipelineRunner<TContext>`, so you don't need to write a custom runner unless you want to customize behavior or simplify dependency resolution.

If you're using custom step interfaces (e.g. to support multiple pipelines), it can be helpful to define a matching custom runner. PipeForge provides a base class, `PipelineRunner<TContext, TStepInterface>`, to make this easy:

```csharp
public interface ISampleContextRunner
    : IPipelineRunner<SampleContext, ISampleContextStep> { }

public class SampleContextRunner
    : PipelineRunner<SampleContext, ISampleContextStep>, ISampleContextRunner { }
```

This approach allows you to resolve your runner via ISampleContextRunner, rather than repeating the full generic signature. When using PipeForge's registration extensions, your custom runner will be registered automatically - no manual wiring required.

## Custom Runners

If you need to extend the default behavior (e.g. to add logging, diagnostics, or custom instrumentation), you can create your own runner by implementing `IPipelineRunner<TContext, TStepInterface>` directly.