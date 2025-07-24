# PipeForge

PipeForge is a lightweight, composable pipeline framework for .NET. It makes step-based processing simple, discoverable, and testable. Inspired by middleware pipelines and modern dependency injection patterns, PipeForge gives you structured control over sequential logic flows - without the ceremony.

Built on the [Chain of Responsibility pattern](https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern), PipeForge promotes loosely coupled design by letting each step decide whether to handle, modify, pass along, or halt execution. This enables flexible control flow, simplified overrides, and easier extension points throughout your processing logic.

[**Documentation**](https://scottoffen.github.io/pipeforge/) | [**Source Code**](https://github.com/scottoffen/pipeforge)

## Features

| | |
|-|-|
| **Dependency Injection** | Steps and services are registered via convenient discovery and registration extension to `IServiceCollection`, allowing resolution through `IServiceProvider`, enabling clean, testable, and composable pipelines. |
| **Asynchronous** | Steps are executed asynchronously with `CancellationToken` support for responsive and scalable operations. |
| **Minimal Config** | Define and run pipelines with minimal boilerplate - ideal for microservices, scripting, and modular business logic. |
| **Fluent Builder** | Configure services, add steps, and customize execution using a fluent API with strong IntelliSense support and minimal boilerplate. |
| **Multiple Pipelines** | Register and execute distinct pipelines that operate on the same context type, each with independent configuration and steps. |
| **Order and Filter** | Steps declare execution order and optional filter tags for conditional inclusion in specific pipelines. |
| **Short-Circuit** | Steps can choose to skip the remainder of the pipeline by not invoking the `next` delegate, enabling conditional flow control and performance optimization. |
| **Lazy Instantiation** | Steps are fetched from the container fresh for each pipeline run and only instantiated if execution reaches them. This avoids unnecessary allocations, honors scoped lifetimes, and improves performance - especially when pipelines exit early. |
