using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace PipeForge.Logging;

internal sealed class MinimalConsoleLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, MinimalConsoleLogger> _loggers = new();

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new MinimalConsoleLogger(name));

    public void Dispose() => _loggers.Clear();
}
