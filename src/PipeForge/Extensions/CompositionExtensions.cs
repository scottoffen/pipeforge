using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PipeForge.Extensions;

internal static class CompositionExtensions
{
    private static readonly string _loggerCategory = "PipeForge";

    /// <summary>
    /// Retrieves the <see cref="ILoggerFactory"/> from the service collection if it has been registered.
    /// If it has not been registered, it will build the service provider to retrieve it.
    /// </summary>
    /// <param name="services"></param>
    internal static ILogger? GetLogger(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var loggerFactory = provider.GetService<ILoggerFactory>();
        return loggerFactory?.CreateLogger(_loggerCategory);
    }
}
