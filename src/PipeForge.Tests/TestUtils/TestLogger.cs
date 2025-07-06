using Microsoft.Extensions.Logging;

namespace PipeForge.Tests.TestUtils;

public class TestLogger : ILogger
{
    public List<TestLogEntry> LogEntries { get; } = new();

    public List<object> Scopes { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        Scopes.Add(state);
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true; // Always enabled for testing purposes
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var logEntry = new TestLogEntry(logLevel, eventId, typeof(TState), exception, formatter(state, exception), state);
        LogEntries.Add(logEntry);
    }

    private class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }
}
