using Microsoft.Extensions.Logging;
using PipeForge.Extensions;

namespace PipeForge;

public static class Pipeline
{
    internal static readonly string MessageInvalidContextType = "The context type '{0}' cannot implement IPipelineStep<>.";

    /// <summary>
    /// Creates a new instance of <see cref="PipelineBuilder{TContext}"/> for the specified context type.
    /// </summary>
    /// <remarks>
    /// This method is used to start building a pipeline for a specific type.
    /// It allows for fluent configuration of pipeline steps.
    /// </remarks>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    public static PipelineBuilder<TContext> CreateFor<TContext>(ILoggerFactory? loggerFactory = null) where TContext : class
    {
        if (typeof(TContext).ImplementsPipelineStep())
        {
            var contextType = typeof(TContext);
            var contextTypename = contextType.FullName ?? contextType.Name;
            throw new ArgumentException(string.Format(MessageInvalidContextType, contextTypename));
        }

        return new PipelineBuilder<TContext>(loggerFactory);
    }
}
