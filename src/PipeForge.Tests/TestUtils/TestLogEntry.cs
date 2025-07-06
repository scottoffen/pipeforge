using Microsoft.Extensions.Logging;

namespace PipeForge.Tests.TestUtils;

public class TestLogEntry
{
    public LogLevel LogLevel { get; }
    public EventId EventId { get; }
    public Type StateType { get; }
    public Exception? Exception { get; }
    public string? Message { get; }
    public object? State { get; }

    public TestLogEntry(LogLevel logLevel, EventId eventId, Type stateType, Exception? exception, string? message, object? state)
    {
        LogLevel = logLevel;
        EventId = eventId;
        State = state;
        StateType = stateType;
        Exception = exception;
        Message = message;
    }
}
