---
sidebar_position: 3
title: Step Discovery
---

# Step Discovery

In order to ensure that pipeline steps are both registered correctly and registered in the correct order with the dependency injection container, use the provided extension method. This method scans for implementations of `IPipelineStep<T>` that are decorated with the `[PipelineStep]` attribute. In addition to steps, this method will also register a pipeline runner of type `IPipelineRunner<T>`.

## Using the Extension Method

When the extension method is called using only the type parameter `T`, it will scan all assemblies in the AppDomain for classes that implement `IPipelineStep<T>` and have the `PipelineStep` attribute that does not specify a filter. It will then register all matching classes in the dependency injection container - along with an instance of `IPipelineRunner<T>` - with a transient service lifetime.

```csharp
services.AddPipelineFor<SampleContext>();
```

The extension method takes optional parameters to change the service lifetime or to include steps for a particular filter.

```csharp
// Add services using a scoped service lifetime
services.AddPipelineFor<SampleContext>(ServiceLifetime.Scoped);

// Include steps explicitly marked with the "Development" filter
service.AddPipelineFor<SampleContext>("Development");

// Use a scoped lifetime and include marked with the "Development" filter
service.AddPipelineFor<SampleContext>(ServiceLifetime.Scoped, "Development");
```

Using this service method will relieve you of the need to register each step individually.

## Register Individual Steps

For advanced scenarios, you can register individual pipeline steps directly. This method takes the step type (not the context type) as the generic parameter, and has an optional parameter for the desired service lifetime. If unspecified, the service lifetime will be transient.

```csharp
public class ManuallyRegisterStep : PipelineStep<StepContext>
{
    // implementation
}

services.AddPipelineStep<ManuallyRegisterStep>(ServiceLifetime.Scoped);
```

Using this extension method will NOT register a corresponding instance of `IPipelineRunner<T>`.

## Conclusion

With your steps discovered and registered, you're ready to run the pipeline.