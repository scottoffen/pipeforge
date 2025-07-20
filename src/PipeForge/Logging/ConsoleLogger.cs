using Microsoft.Extensions.Logging;

namespace PipeForge.Logging;

internal sealed class MinimalConsoleLogger : ILogger
{
    private readonly string _categoryName;

    public MinimalConsoleLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        Console.WriteLine($"[{logLevel}] {_categoryName}: {message}");

        if (exception is not null)
        {
            Console.WriteLine(exception);
        }
    }
}
