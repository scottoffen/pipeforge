
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
