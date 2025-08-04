---
sidebar_position: 3
title: Pipeline Runners
---

A pipeline runner is responsible for executing a pipeline by resolving and invoking each registered step in order. It handles lazy instantiation, short-circuiting, cancellation, and exceptions during execution.

## Default Runner Interfaces and Implementation

All runners implement the `IPipelineRunner<TContext, TStepInterface>` interface, where `TStepInterface` must implement `IPipelineStep<TContext>`. `TStepInterface` is typically used to distinguish steps when multiple pipelines share the same context type. If your application uses only one pipeline per context, you can use the simplified `IPipelineRunner<TContext>` interface, which is easier to register and resolve from the dependency injection container.

PipeForge provides default implementations for both interfaces. You only need a custom runner if you want to override execution behavior or manually manage dependency resolution.

## Custom Runner Interfaces

When using custom step interfaces (e.g. for multiple pipelines with the same context), you can define a corresponding runner interface without implementing it—PipeForge will generate the implementation automatically in supported environments. This simplifies DI registration and resolution by avoiding generic type repetition.

```csharp title="Create the interface"
public interface ISampleContextRunner
    : IPipelineRunner<SampleContext, ISampleContextStep> { }
```

```csharp title="Register the interface"
builder.Services.AddPipeline<SampleContext, ISampleContextStep, ISampleContextRunner>();
```

```csharp title="Resolve the interface"
public class MyService
{
    private readonly ISampleContextRunner _runner;

    public MyService(ISampleContextRunner runner)
    {
        _runner = runner;
    }
}
```

:::tip

In .NET 5.0 and later (excluding AOT), PipeForge generates the runner implementation automatically—you only need to define the interface.

:::

## Custom Runner Implementation

The following table summarizes when you need to provide a **concrete implementation**:

| Target Environment | Interface Only | Concrete Implementation Required |
| ------------------ | -------------- | -------------------------------- |
| .NET 5.0 or higher | ✅              | ❌                                |
| Native AOT         | ❌              | ✅                                |
| .NET Standard 2.0  | ❌              | ✅                                |

If you’re targeting .NET Standard 2.0 or Native AOT, you must provide a concrete implementation. PipeForge makes this easy by providing a base class, `PipelineRunner<TContext, TStepInterface>`:

```csharp title="Custom interface implementation"
public class SampleContextRunner
    : PipelineRunner<SampleContext, ISampleContextStep>, ISampleContextRunner
{
    public SampleContextRunner(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
```

This approach allows you to register and resolve your runner via `ISampleContextRunner`, same as before.

:::tip

 To customize behavior (e.g. logging, diagnostics, instrumentation), implement `IPipelineRunner<TContext, TStepInterface>` directly or subclass `PipelineRunner<TContext, TStepInterface>`.

:::