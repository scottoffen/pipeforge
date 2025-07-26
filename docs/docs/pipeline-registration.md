---
sidebar_position: 4
title: Pipeline Registration
---

# Pipeline Registration

PipeForge uses dependency injection to resolve both pipeline steps and their runners, and leverages lazy instantiation to avoid constructing steps that will never be executed - either due to short-circuiting, cancellation tokens, or runtime exceptions.

This section describes how to register pipelines using the built-in extension methods provided by PipeForge.

## Register a Pipeline

The most common way to register a pipeline is by using the `AddPipeline` extension methods on `IServiceCollection`. These methods automatically discover and register all pipeline steps for a given context and step interface by scanning the provided assemblies. An appropriate runner is also registered, allowing you to resolve and execute the pipeline easily.

### Method Signature

```csharp
public static IServiceCollection AddPipeline<TContext, TStepInterface, TRunnerInterface>(
    this IServiceCollection services,
    IEnumerable<Assembly> assemblies,
    ServiceLifetime lifetime,
    string[]? filters)
    where TContext : class
    where TStepInterface : IPipelineStep<TContext>
    where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
{ }
```

### Parameters

| Parameter          | Required |Description                                                                                                 |
| ------------------ | :------: |----------------------------------------------------------------------------------------------------------- |
| `TContext`         |  ✅       | The context class shared across all steps in the pipeline.                                                 |
| `TStepInterface`   |          | The interface used to identify pipeline steps. Defaults to `IPipelineStep<TContext>`.                      |
| `TRunnerInterface` |          | The interface used to resolve the pipeline runner. Defaults to `IPipelineRunner<TContext, TStepInterface>`.|
| `assemblies`       |          | Assemblies to scan for steps. If not provided, `AppDomain.CurrentDomain` is used.                          |
| `lifetime`         |          | The DI lifetime for steps and the runner. Defaults to `ServiceLifetime.Transient`.                         |
| `filters`          |          | Optional filters to limit which steps are registered.                                                      |


### Examples

Here are some examples of how to use the various overloads of `AddPipeline`:

``` csharp
// Minimal registration using default step and runner interfaces
services.AddPipeline<SampleContext>();

// Register with a custom step interface
services.AddPipeline<SampleContext, ISampleContextStep>();

// Register with a custom step and custom runner interface
services.AddPipeline<SampleContext, ISampleContextStep, ISampleContextRunner>();

// Register with specific assemblies and a scoped lifetime
services.AddPipeline<SampleContext, ISampleContextStep>(
    new[] { typeof(MyStep).Assembly },
    ServiceLifetime.Scoped);

// Register with multiple filters
services.AddPipeline<SampleContext, ISampleContextStep>(
    new[] { typeof(MyStep).Assembly },
    new[] { "Development", "Testing" });
```

## Register a Single Step

If you prefer to manually register individual steps, or want fine-grained control over registration, use the `AddPipelineStep` extension method.

:::danger[Warning]

When registering steps individually:
- The `PipelineStep` attribute on the step is neither required nor used.
- No pipeline runner is registered. You will need to register it yourself.

:::

### Method Signature

```csharp
public static IServiceCollection AddPipelineStep<TStep, TStepInterface>(
    this IServiceCollection services,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
    where TStep : class, TStepInterface
    where TStepInterface : class, IPipelineStep
{ }
```

### Parameters

| Parameter        | Description                                                                                     | Required |
| ---------------- | ----------------------------------------------------------------------------------------------- | -------- |
| `TStep`          | The concrete step class to register.                                                            | ✅        |
| `TStepInterface` | The interface used to register the step. Defaults to `IPipelineStep<TContext>` if not provided. | ❌        |
| `lifetime`       | The DI lifetime for the step. Defaults to `ServiceLifetime.Transient`.                          | ❌        |

If `TStepInterface` is not provided, the method will attempt to infer the context type from `TStep`.
