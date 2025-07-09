---
sidebar_position: 1
title: Getting Started
---

# Welcome to PipeForge

PipeForge is a lightweight, composable, lazy-instantiation pipeline framework for .NET. It simplifies step-based processing while remaining discoverable and testable. Inspired by middleware pipelines and modern dependency injection patterns, PipeForge gives you structured control over sequential logic flows - without the ceremony.

Pipelines operate on a specific class known as the **context**. Each pipeline step is a discrete unit of work, written in code and annotated with metadata indicating its order and (optional) environment. These steps are lazily instantiated and executed in sequence by the pipeline runner.

At any point, the pipeline can **short-circuit**, halting execution — and preventing the instantiation of any remaining steps.

## Sample Context

For the purposes of this documentation, the following sample context will be used.

```csharp title="SampleContext.cs"
public class SampleContext
{
    private readonly List<string> _steps = new();

    public void AddStep(string stepName)
    {
        if (string.IsNullOrWhiteSpace(stepName))
        {
            throw new ArgumentException("Step name cannot be null or whitespace.", nameof(stepName));
        }

        _steps.Add(stepName);
    }

    public int StepCount => _steps.Count;

    public override string ToString()
    {
        return string.Join(",", _steps);
    }
}
```

This context allows us to:
- Track pipeline progress via AddStep()
- Print execution history using ToString()
- Assert how many steps ran using StepCount
- Simulate errors by passing invalid step names

## Installation

PipeForge is available on [NuGet.org](https://www.nuget.org/packages/PipeForge/) and can be installed using a NuGet package manager or the .NET CLI.

PipeForge targets `.NET Standard 2.0` for broad compatibility, and also multi-targets `.NET 5.0` to take advantage of modern runtime features where available.