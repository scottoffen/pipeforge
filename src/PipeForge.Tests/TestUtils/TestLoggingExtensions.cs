using Microsoft.Extensions.Logging;

namespace PipeForge.Tests.TestUtils;

public static class TestLoggerExtensions
{
    public static TestLogEntry ShouldContainMessage(this TestLogger logger, string expectedMessage, LogLevel? level = null)
    {
        var matching = logger.LogEntries
            .Where(e => e.Message?.Contains(expectedMessage) == true)
            .ToList();

        if (level.HasValue)
            matching = matching.Where(e => e.LogLevel == level.Value).ToList();

        matching.Count.ShouldBeGreaterThan(0, $"Expected to find log message containing \"{expectedMessage}\" at level {level?.ToString() ?? "(any)"}");

        return matching.First();
    }

    public static TestLogEntry ShouldContainMessageMatching(this TestLogger logger, Func<string, bool> predicate, LogLevel? level = null)
    {
        var matching = logger.LogEntries
            .Where(e => e.Message != null && predicate(e.Message))
            .ToList();

        if (level.HasValue)
            matching = matching.Where(e => e.LogLevel == level.Value).ToList();

        matching.Count.ShouldBeGreaterThan(0, "Expected to find a log message matching predicate");

        return matching.First();
    }

    public static void ShouldHaveLoggedError(this TestLogger logger, Type expectedExceptionType)
    {
        logger.LogEntries
            .Any(e => e.LogLevel == LogLevel.Error && e.Exception?.GetType() == expectedExceptionType)
            .ShouldBeTrue($"Expected an error log entry with exception type {expectedExceptionType.Name}");
    }

    public static object ShouldHaveScopeContaining(this TestLogger logger, string key, object? expectedValue)
    {
        var match = logger.Scopes
            .OfType<IEnumerable<KeyValuePair<string, object>>>()
            .FirstOrDefault(scope => scope.Any(kv => kv.Key == key && Equals(kv.Value, expectedValue)));

        match.ShouldNotBeNull($"Expected a logging scope containing key '{key}' with value '{expectedValue}'");

        return match!;
    }

    public static object ShouldHaveScopeMatching(this TestLogger logger, Func<IEnumerable<KeyValuePair<string, object>>, bool> predicate)
    {
        var match = logger.Scopes
            .OfType<IEnumerable<KeyValuePair<string, object>>>()
            .FirstOrDefault(predicate);

        match.ShouldNotBeNull("Expected a logging scope matching the specified predicate");

        return match!;
    }
}

