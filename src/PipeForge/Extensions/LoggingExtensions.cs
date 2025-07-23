using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PipeForge.Extensions;

[ExcludeFromCodeCoverage]
internal static class LoggingExtensions
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

    /// <summary>
    /// Logs the number of steps registered for a specific type.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="count"></param>
    /// <param name="typeName"></param>
    internal static void LogStepsRegistered(this ILogger? logger, int count, string typeName)
    {
        if (count == 0)
        {
            logger?.LogWarning(PipelineRegistration.MessageNoStepsFound, typeName);
        }
        else
        {
            logger?.LogInformation(PipelineRegistration.MessageNumberStepsFound, count, typeName);
        }
    }
}
