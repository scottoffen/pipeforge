/// <summary>
/// Executes a sequence of pipeline steps for a given context type
/// </summary>
/// <typeparam name="T">The type of context passed through the pipeline</typeparam>
public interface IPipelineRunner<T>
{
    /// <summary>
    /// Runs the pipeline steps in order using the provided context
    /// </summary>
    /// <param name="context">The context object to be processed by the pipeline</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation</param>
    Task ExecuteAsync(T context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a JSON array describing the steps in the pipeline
    /// </summary>
    string Describe();
#if !NETSTANDARD2_0
    /// <summary>
    /// Returns the JSON schema representing the structure of the pipeline step metadata
    /// </summary>
    string DescribeSchema();
#endif
}
