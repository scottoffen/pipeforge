
using System.Diagnostics.CodeAnalysis;

namespace PipeForge;

[ExcludeFromCodeCoverage]
internal sealed class DefaultPipelineRunner<TContext, TStepInterface> : PipelineRunner<TContext, TStepInterface>, IPipelineRunner<TContext, TStepInterface>
    where TContext : class
    where TStepInterface : IPipelineStep<TContext>
{
    public DefaultPipelineRunner(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}

[ExcludeFromCodeCoverage]
internal sealed class DefaultPipelineRunner<TContext> : PipelineRunner<TContext, IPipelineStep<TContext>>, IPipelineRunner<TContext>
    where TContext : class
{
    public DefaultPipelineRunner(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
