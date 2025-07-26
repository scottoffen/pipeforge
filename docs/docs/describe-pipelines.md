---
sidebar_position: 8
title: Describing Pipelines
---

PipeForge provides built-in support for inspecting pipeline structure and behavior at runtime. This is especially useful for observability, testing, documentation, and UI tooling.

## Describe()

The `Describe()` method on `IPipelineRunner<T>` outputs a JSON array representing each registered step. It includes key metadata such as name, order, and short-circuit configuration.

:::warning[Side Effects]

This method **instantiates all steps**, triggering constructor injection and service resolution. Use it only when such side effects are acceptable.

:::

### Output Format

| Field                   | Description                                             |
| ----------------------- | ------------------------------------------------------- |
| `Order`                 | Step's position in the execution sequence               |
| `Name`                  | Value of the step’s `Name` property                     |
| `Description`           | Value of the step’s `Description`, if provided          |
| `MayShortCircuit`       | Indicates if the step may halt pipeline execution       |
| `ShortCircuitCondition` | Explanation of the short-circuit trigger, if applicable |

### Example Output

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

:::info[A Note About Order]

The `Order` shown in the output reflects the **actual runtime execution sequence**, not the numeric value assigned via the `[PipelineStep]` attribute. For example, two steps with attributes `Order = 3` and `Order = 4` will appear in the output as `Order = 0` and `Order = 1` if they are the only steps registered.

:::

### Use Cases

* Logging pipeline structure for observability
* Generating runtime or admin UI documentation
* Verifying step order and metadata in unit tests

If you need to inspect step configuration without triggering instantiation, consider capturing step metadata during registration or design time.

## DescribeSchema()

The `DescribeSchema()` method returns a [JSON Schema v7](https://json-schema.org/specification.html) document that describes the metadata shape of a pipeline step. This is ideal for tools that visualize or validate pipeline structures.

### When to Use

* Exposing step definitions through APIs or dashboards
* Powering UI editors or metadata-driven configuration
* Documenting expected step shape for developers
* Validating configuration or orchestration input

### Schema Fields

| Property                | Type    | Description                                         |
| ----------------------- | ------- | --------------------------------------------------- |
| `Order`                 | integer | Execution order of the step (inferred at runtime)   |
| `Name`                  | string  | Display name of the step                            |
| `Description`           | string  | Optional summary of the step’s purpose              |
| `MayShortCircuit`       | boolean | Indicates if the step may halt execution early      |
| `ShortCircuitCondition` | string  | Optional explanation of the short-circuit condition |

Only `Order` is required. All other fields are optional and serve documentation, inspection, or visualization purposes.

### Example Output

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": "PipelineStep",
  "type": "object",
  "properties": {
    "Order": { "type": "integer", "description": "Execution order of the step (inferred)" },
    "Name": { "type": "string", "description": "Display name of the step" },
    "Description": { "type": "string", "description": "Optional description of the step" },
    "MayShortCircuit": { "type": "boolean", "description": "Whether the step may halt pipeline execution early" },
    "ShortCircuitCondition": { "type": "string", "description": "Explanation of the short-circuit condition, if any" }
  },
  "required": ["Order"]
}
```

Use this schema to build validation layers, generate dynamic forms, or publish step definitions in API documentation.
