using System.Diagnostics.CodeAnalysis;

namespace PipeForge.Adapters.Diagnostics;

[ExcludeFromCodeCoverage]
internal static class PipelineDiagnosticsFactory
{
    /// <summary>
    /// Returns an implementation of IPipelineDiagnostics for the given class T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IPipelineDiagnostics<T> Create<T>() where T : class
    {
#if NETSTANDARD2_0
        return new DiagnosticListenerProvider<T>();
#else
        return new ActivitySourceProvider<T>();
#endif
    }
}
