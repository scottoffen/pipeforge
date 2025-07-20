# PipeForge

PipeForge is a lightweight, composable pipeline framework for .NET. It makes step-based processing simple, discoverable, and testable. Inspired by middleware pipelines and modern dependency injection patterns, PipeForge gives you structured control over sequential logic flows - without the ceremony.

Built on the [Chain of Responsibility pattern](https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern), PipeForge promotes loosely coupled design by letting each step decide whether to handle, modify, pass along, or halt execution. This enables flexible control flow, simplified overrides, and easier extension points throughout your processing logic.

> [!TIP]
> While PipeForge is fundamentally a pipeline framework, it can also serve as the foundation for higher-level workflows. These workflows are built by composing individual pipeline steps that handle branching, retries, fallbacks, and decision logic — making it ideal for orchestrating complex processes like AI chains, data enrichment, or multi-stage validation.

[**Documentation**](https://scottoffen.github.io/pipeforge/) | [**Source Code**](https://github.com/scottoffen/pipeforge)

## ✅ Features

**🧩 Supports Dependency Injection**  
Steps and services are registered via convenient discovery and registration extension to `IServiceCollection`, allowing resolution through `IServiceProvider`, enabling clean, testable, and composable pipelines.

**⏱️ Asynchronous Execution**  
Steps are executed asynchronously with `CancellationToken` support for responsive and scalable operations.

**🛠️ Minimal Configuration**  
Define and run pipelines with minimal boilerplate - ideal for microservices, scripting, and modular business logic.

**🔗 Fluent Pipeline Builder**  
Configure services, add steps, and customize execution using a fluent API with strong IntelliSense support and minimal boilerplate.

**🧬 Multiple Pipelines for the Same Context**  
Register and execute distinct pipelines that operate on the same context type, each with independent configuration and steps.

**🎛️ Step-Level Ordering and Filtering**  
Steps declare execution order and optional filter tags for conditional inclusion in specific pipelines.

**⚙️ Configurable Service Lifetime**  
Steps can be registered with a custom `ServiceLifetime` (`Transient`, `Scoped`, or `Singleton`), defaulting to `Transient` for lightweight, stateless operation.

**💡 Delegate-Based Step Registration**  
Add steps inline with simple delegate functions - no need to define full classes unless desired.

**🚪 Early Exit (Short-Circuiting) Support**  
Steps can choose to skip the remainder of the pipeline by not invoking the `next` delegate, enabling conditional flow control and performance optimization.

**💤 Lazy and Per-Execution Step Instantiation**  
Steps are fetched from the container fresh for each pipeline run and only instantiated if execution reaches them. This avoids unnecessary allocations, honors scoped lifetimes, and improves performance - especially when pipelines exit early.

**🧾 Built-In Logging Integration**  
Integrates with `ILogger<T>` or a custom `ILoggerFactory`, with sensible defaults for console output.

**🧪 Composable and Testable**  
Pipelines can be easily unit tested or composed within other services, making them great for orchestration or domain-driven logic.

**🔍 Optional Schema and Diagnostics Support**  
Generate a structured description of pipeline steps for debugging or documentation. Hook into diagnostics to monitor execution time, order, and outcomes for each step.

## 🎯 Use Cases

**🧩 Middleware-Style Request Processing**  
Building lightweight middleware for jobs, requests, or messages? PipeForge gives you all the structure of an ASP.NET pipeline without the hosting overhead. Plug in validation, enrichment, policy checks, and post-processing steps that run cleanly in sequence, and short-circuit when they should.

**⚙️ DevOps and Automation Pipelines**  
From deployment checks to file transforms to notification triggers, DevOps automation often follows clear, repeatable logic flows. PipeForge lets you express those pipelines clearly and predictably - each step knows what to do, when to do it, and when to step aside. No YAML templating required.

**🛡️ Security and Auditing Pipelines**  
Enforce policy, detect anomalies, redact sensitive data, and log events - all in one traceable flow. With PipeForge, each responsibility lives in its own step, making compliance and observability straightforward instead of bolted-on. Early exit ensures fast rejection of non-compliant input before deeper processing.

**📦 ETL and Data Processing Pipelines**  
Tired of turning every data flow into a procedural tangle? With PipeForge, you can express each stage of your ETL process - validation, transformation, enrichment, persistence - as discrete, reusable steps. Pipelines stay focused and maintainable, even as your processes grow.

**🤖 LLM and AI Integration Pipelines**  
LLM-based workflows can be fragile and stateful - PipeForge helps you sequence prompts, formatters, filters, and fallback handlers without turning your orchestration into spaghetti. Build resilient, testable AI pipelines that treat each step as a first-class citizen.

**🔧 Business Logic and Domain Orchestration**  
No more brittle switch statements, tangled conditionals, or monolithic handlers. PipeForge helps you orchestrate complex business rules with clarity - one decision, transformation, or side effect at a time. Your logic becomes readable, testable, and easier to evolve when each step has a single responsibility.
