---
sidebar_position: 6
title: Describing Pipelines
---

# Describing Pipelines

The `Describe()` method on `IPipelineRunner<T>` is used to inspect and document the pipeline configuration at runtime. It returns a JSON string describing each registered pipeline step.

This method is useful for diagnostics, tooling, and dynamically displaying pipeline behavior in user interfaces or logs - but only if you've taken the time to add the necessary metadata to your step classes.

## Output Format

The JSON output contains an array of objects, each representing a pipeline step with the following fields:

* `Order`: The zero-based order in which the step appears in the pipeline
* `Name`: The value of the step's `Name` property
* `Description`: The step's `Description`, if defined
* `MayShortCircuit`: A boolean indicating whether the step might short-circuit execution
* `ShortCircuitCondition`: The value of the step's `ShortCircuitCondition`, if any

Example output:

```json
[
  {
    "Order": 0,
    "Name": "ValidateInput",
    "Description": "Checks that required values are present",
    "MayShortCircuit": true,
    "ShortCircuitCondition": "Invalid input"
  },
  {
    "Order": 1,
    "Name": "TransformData",
    "Description": "Prepares data for downstream steps",
    "MayShortCircuit": false,
    "ShortCircuitCondition": null
  }
]
```

:::warning Warning

The `Order` value in the JSON output represents the execution order of the steps. This value is assigned based on the order in which the steps will be executed, and it may differ from the `Order` specified in each step's `PipelineStep` attribute.

For example, if you have only two steps with `PipelineStep(Order = 3)` and `PipelineStep(Order = 4)`, the JSON output will show `Order` values of `0` and `1`, respectively - reflecting their relative execution sequence, not their original attribute values.

:::

## Instantiation Behavior

Calling `Describe()` **will instantiate all steps** in the pipeline by accessing their `Lazy<T>` wrappers. This may result in constructor injection or other side effects associated with instantiating the step class. Use this method only when you are prepared for that overhead.

## Use Cases

* Logging pipeline structure for observability
* Generating runtime documentation or UI
* Verifying step order and metadata during tests or builds

If you need to inspect step configuration without triggering instantiation, consider maintaining parallel metadata or restricting usage of `Describe()` to controlled environments.

## Conclusion

Use `Describe()` to introspect your pipeline at runtime, but be aware that it will force full resolution of every registered step.
