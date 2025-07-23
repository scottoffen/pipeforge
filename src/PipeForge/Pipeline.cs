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
    public static PipelineBuilder<TContext> CreateFor<TContext>(ILoggerFactory? loggerFactory = null)
        where TContext : class
    {
        var contextType = typeof(TContext);
        if (contextType.ImplementsPipelineStep())
        {
            throw new ArgumentException(string.Format(MessageInvalidContextType, contextType.GetTypeName()));
        }

        return new PipelineBuilder<TContext>();
    }
}
