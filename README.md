# PipeForge

[![docs](https://img.shields.io/badge/docs-github.io-blue)](https://scottoffen.github.io/pipeforge)
[![NuGet](https://img.shields.io/nuget/v/PipeForge)](https://www.nuget.org/packages/PipeForge/)
[![MIT](https://img.shields.io/github/license/scottoffen/pipeforge?color=blue)](./LICENSE)
[![Target1](https://img.shields.io/badge/netstandard-2.0-blue)](https://learn.microsoft.com/en-us/dotnet/standard/frameworks)
[![Target1](https://img.shields.io/badge/dotnet-5.0-blue)](https://learn.microsoft.com/en-us/dotnet/standard/frameworks)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-blue.svg)](code_of_conduct.md)

PipeForge is a lightweight, composable pipeline framework for .NET. It makes step-based processing simple, discoverable, and testable. Inspired by middleware pipelines and modern dependency injection patterns, PipeForge gives you structured control over sequential logic flows - without the ceremony.

## Installation

PipeForge is available on [NuGet.org](https://www.nuget.org/packages/PipeForge/) and can be installed using a NuGet package manager or the .NET CLI.

## Usage

Pipelines are designed to operate on specific class, referred to as the **context**. Multiple pipeline steps are created in code to operate on that context. Steps are annotated with an attribute indicating the order in which they should be executed. Finally, the pipeline runner is given an instance of the context to run against.

The following example uses dependency injection, and is the recommended approach to using PipeForge. For more advanced scenarios, see the full [documentation](https://scottoffen.github.io/pipeforge).

> [!NOTE]
> I'm suffixing my context class with the word `Context`, and my steps with the word `Step` for demonstration purposes only.

### Create Your Context

```csharp
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
        return string.Join("", _steps);
    }
}
```

### Create Your Steps

```csharp
[PipelineStep(1)]
public class HelloStep : PipelineStep<SampleContext>
{
    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("Hello");
        await next(context, cancellationToken);
    }
}

[PipelineStep(2)]
public class WorldStep : PipelineStep<SampleContext>
{
    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("World");
        await next(context, cancellationToken);
    }
}

[PipelineStep(3)]
public class PunctuationStep : PipelineStep<SampleContext>
{
    public override async Task InvokeAsync(SampleContext context, PipelineDelegate<SampleContext> next, CancellationToken cancellationToken = default)
    {
        context.AddStep("1");
        await next(context, cancellationToken);
    }
}
```

### Use Dependency Injection

The extension method will discover and register all steps for the given `T` context, as well as register an instance of `IPipelineRunner<T>` that can be injected into services.

```csharp
services.AddPipelineFor<SampleContext>();
```

### Execute Pipeline

Get an instance of `IPipelineRunnere<SampleContext>` from your dependency injection container.

```csharp
public class SampleService
{
    private readonly IPipelineRunner<SampleContext> _pipelineRunner;

    public SampleService(IPipelineRunner<SampleContext> pipelineRunner)
    {
        _pipelineRunner = pipelineRunner;
    }

    public async Task RunPipeline(SampleContext context, CancellationToken? token = default)
    {
        await _pipelineRunner.ExecuteAsync(context, token);
        var result = context.ToString();
        // result = "HelloWorld!"
    }
}
```

## Support

- Check out the project documentation https://scottoffen.github.io/pipeforge.

- Engage in our [community discussions](https://github.com/scottoffen/pipeforge/discussions) for Q&A, ideas, and show and tell!

- Have a question you can't find an answer for in the documentation or discussions? You can ask your questions on [StackOverflow](https://stackoverflow.com) using [#pipeforge](https://stackoverflow.com/questions/tagged/pipeforge?sort=newest). Make sure you include the version of PipeForge you are using, the platform you using it on, code samples and any specific error messages you are seeing.

- **Issues created to ask "how to" questions will be closed.**

## Contributing

We welcome contributions from the community! In order to ensure the best experience for everyone, before creating an issue or submitting a pull request, please see the [contributing guidelines](CONTRIBUTING.md) and the [code of conduct](CODE_OF_CONDUCT.md). Failure to adhere to these guidelines can result in significant delays in getting your contributions included in the project.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/scottoffen/pipeforge/releases).

## Test Coverage

You can generate and open a test coverage report by running the following command in the project root:

```bash
pwsh ./test-coverage.ps1
```

> [!NOTE]
> This is a [Powershell](https://learn.microsoft.com/en-us/powershell/) script. You must have Powershell installed to run this command.
> The command depends on the global installation of the dotnet tool [ReportGenerator](https://www.nuget.org/packages/ReportGenerator).

## License

PipeForge is licensed under the [MIT](./LICENSE) license.

## Using PipeForge? We'd Love To Hear About It!

Few thing are as satisfying as hearing that your open source project is being used and appreciated by others. Jump over to the discussion boards and [share the love](https://github.com/scottoffen/pipeforge/discussions)!