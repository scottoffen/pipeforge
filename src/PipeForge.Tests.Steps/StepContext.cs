using System;
using System.Collections.Generic;

namespace PipeForge.Tests.Steps;

public class StepContext
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
