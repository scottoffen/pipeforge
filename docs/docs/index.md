---
sidebar_position: 1
title: Getting Started
---

# Welcome to PipeForge

PipeForge is a lightweight, composable pipeline framework for .NET that makes step-based workflows easy to build, test, and maintain. With lazy instantiation and modern dependency injection, it gives you structured control over execution flow — without the heavy scaffolding of base classes, rigid lifecycles, or tightly coupled framework logic. Inspired by the simplicity of middleware pipelines, PipeForge favors clear, minimal structure over hidden complexity.

Each pipeline operates on a specific class called the **context**, which flows through each step in sequence. Steps are discrete units of work, written as regular code and annotated with metadata to define their order and optional filters. They’re lazily instantiated — only created when needed — and executed by the pipeline runner.

At any point, a step can **short-circuit** the pipeline, halting further execution and preventing the instantiation of remaining steps.

## Sample Context

For the purposes of this documentation, the following sample context will be used.

```csharp title="SampleContext.cs"
public class SampleContext
{
    public readonly List<string> Steps = new();

    public void AddStep(string stepName)
    {
        if (string.IsNullOrWhiteSpace(stepName))
        {
            throw new ArgumentException("Step name cannot be null or whitespace.", nameof(stepName));
        }

        Steps.Add(stepName);
    }

    public int StepCount => Steps.Count;

    public override string ToString()
    {
        return string.Join(",", Steps);
    }
}
```

This context allows us to:
- Track pipeline progress via `AddStep()`
- Evaluate the order and number of step executions
- Print step execution history using `ToString()`
- Simulate errors by passing null or empty step names

## Installation

PipeForge is available on [NuGet.org](https://www.nuget.org/packages/PipeForge/) and can be installed using a NuGet package manager or the .NET CLI.

PipeForge targets `.NET Standard 2.0` for broad compatibility, and also multi-targets `.NET 5.0` to take advantage of modern runtime features where available.