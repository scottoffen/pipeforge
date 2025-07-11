---
sidebar_position: 8
title: Diagnostics
---

# Diagnostics

PipeForge includes built-in diagnostics via the [`System.Diagnostics.DiagnosticSource`](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.diagnosticsource) API. This allows you to observe pipeline execution without modifying your steps.

## Emitted Events

The pipeline emits the following diagnostic events for each step:

| Event Name           | Description                                                      |
| -------------------- | ---------------------------------------------------------------- |
| `PipelineStep.Start` | Emitted **before** a step is invoked. Payload includes the step. |
| `PipelineStep.Stop`  | Emitted **after** a step completes. Payload includes the step.   |

All events are emitted under a listener name in the format:

```
PipeForge.PipelineRunner<TContext>
```

You can subscribe to these events to monitor or trace execution flow.

## Example: Listening for Events

Here's an example of a diagnostic listener that logs every start and stop event:

```csharp
using System.Diagnostics;

var listener = new DiagnosticListener("PipeForge.PipelineRunner<SampleContext>");
listener.Subscribe(new Observer((name, payload) =>
{
    Console.WriteLine($"{name}: {payload}");
}));
```

## Integration Tips

* You can use this mechanism to generate timing metrics, debug issues, or visualize step execution.
* Diagnostic events are low-overhead and safe to leave enabled in production.
* Combine this with the `Describe()` method for a full picture of pipeline structure and execution behavior.

## Conclusion

PipeForge diagnostics give you deep visibility into pipeline execution with minimal effort. Whether you're debugging a failing step or building runtime instrumentation, the diagnostics hooks are ready to help.
