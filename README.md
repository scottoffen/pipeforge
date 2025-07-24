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

## Usage and Support

- Check out the project documentation https://scottoffen.github.io/pipeforge.

- Engage in our [community discussions](https://github.com/scottoffen/pipeforge/discussions) for Q&A, ideas, and show and tell!

- Have a question you can't find an answer for in the documentation or discussions? You can ask your questions on [StackOverflow](https://stackoverflow.com) using [#pipeforge](https://stackoverflow.com/questions/tagged/pipeforge?sort=newest). Make sure you include the version of PipeForge you are using, the platform you using it on, code samples and any specific error messages you are seeing.

- **Issues created to ask "how to" questions will be closed.**

## Use Cases

While PipeForge is fundamentally a pipeline framework, it can also serve as the foundation for higher-level workflows. These workflows are built by composing individual pipeline steps that handle branching, retries, fallbacks, and decision logic — making it ideal for orchestrating complex processes like AI chains, data enrichment, or multi-stage validation.

| | |
|-|-|
| Middleware-Style Request Processing | Build lightweight, modular request pipelines similar to ASP.NET middleware, without requiring a full web host. |
| DevOps and Automation Pipelines | Express deployment checks, file transforms, and system hooks as repeatable, testable steps. |
| Security and Auditing Pipelines | Enforce policies, redact sensitive data, and log events in a structured, traceable flow. |
| ETL and Data Processing Pipelines | Break down validation, transformation, and persistence into clean, maintainable processing steps. |
| LLM and AI Workflows | Orchestrate prompt generation, model calls, fallback handling, and response parsing using composable pipelines. |
| Business Logic and Domain Orchestration | Replace brittle `if` chains and nested logic with clearly structured rule execution and orchestration flows. |


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