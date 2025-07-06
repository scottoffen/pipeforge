using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace PipeForge.Tests.PipelineRunner;

public class DiagnosticTests
{
    private class TestContext { }

    private class TestStep : IPipelineStep<TestContext>
    {
        public string Name => "TestStep";
        public string Description => "Test Step Description";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            return next(context, cancellationToken);
        }
    }

    private class ShortCircuitStep : IPipelineStep<TestContext>
    {
        public string Name => "ShortCircuit";
        public string Description => "Short-circuits the pipeline";
        public bool MayShortCircuit => true;
        public string? ShortCircuitCondition => "Always";

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask; // skips `next`
        }
    }

    private class FailingStep : IPipelineStep<TestContext>
    {
        public string Name => "Exploder";
        public string Description => "Throws for testing";
        public bool MayShortCircuit => false;
        public string? ShortCircuitCondition => null;

        public Task InvokeAsync(TestContext context, PipelineDelegate<TestContext> next, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Kaboom");
        }
    }

    [Fact]
    public async Task EmitsStartAndStopEvents()
    {
        var events = new List<(string, object)>();
        var expectedListenerName = $"PipeForge.PipelineRunner<{typeof(TestContext).Name}>";

        using var allListenerSubscription = DiagnosticListener.AllListeners.Subscribe(
            new FilteringListener(expectedListenerName,
                (name, payload) => events.Add((name, payload)),
                (name, _, _) => name == "PipelineStep.Start" || name == "PipelineStep.Stop"));

        var runner = new PipelineRunner<TestContext>(
            new[] { new Lazy<IPipelineStep<TestContext>>(() => new TestStep()) },
            new NullLoggerFactory());

        await runner.ExecuteAsync(new TestContext());

        events.Count.ShouldBe(2);
        events[0].Item1.ShouldBe("PipelineStep.Start");
        events[1].Item1.ShouldBe("PipelineStep.Stop");

        (events[0].Item2.ToString() ?? string.Empty).ShouldContain("TestStep");
        (events[1].Item2.ToString() ?? string.Empty).ShouldContain("TestStep");
    }

    [Fact]
    public async Task EmitsShortCircuitMetadata()
    {
        var events = new List<(string, object)>();
        var expectedListenerName = $"PipeForge.PipelineRunner<{typeof(TestContext).Name}>";

        using var allListenerSubscription = DiagnosticListener.AllListeners.Subscribe(
            new FilteringListener(expectedListenerName,
                (name, payload) => events.Add((name, payload)),
                (name, _, _) => name == "PipelineStep.Start" || name == "PipelineStep.Stop"));

        var runner = new PipelineRunner<TestContext>(
            new[] { new Lazy<IPipelineStep<TestContext>>(() => new ShortCircuitStep()) },
            new NullLoggerFactory());

        await runner.ExecuteAsync(new TestContext());

        events.Count.ShouldBe(2);
        (events[1].Item2.ToString() ?? string.Empty).ShouldContain("short_circuited = True", Case.Insensitive);
    }

    [Fact]
    public async Task EmitsExceptionMetadata()
    {
        var events = new List<(string, object)>();
        var expectedListenerName = $"PipeForge.PipelineRunner<{typeof(TestContext).Name}>";

        using var allListenerSubscription = DiagnosticListener.AllListeners.Subscribe(
            new FilteringListener(expectedListenerName,
                (name, payload) =>
                {
                    if (name == "PipelineStep.Exception")
                        events.Add((name, payload));
                },
                (name, _, _) => name == "PipelineStep" || name == "PipelineStep.Exception"));

        var runner = new PipelineRunner<TestContext>(
            new[] { new Lazy<IPipelineStep<TestContext>>(() => new FailingStep()) },
            new NullLoggerFactory());

        var ex = await Should.ThrowAsync<PipelineExecutionException<TestContext>>(async () =>
        {
            await runner.ExecuteAsync(new TestContext());
        });

        events.Count.ShouldBe(1);
        events[0].Item1.ShouldBe("PipelineStep.Exception");
        (events[0].Item2.ToString() ?? string.Empty).ShouldContain("Kaboom");
        (events[0].Item2.ToString() ?? string.Empty).ShouldContain("Exploder");
    }


    private class FilteringListener : IObserver<DiagnosticListener>, IDisposable
    {
        private readonly string _targetName;
        private readonly Action<string, object> _onNext;
        private readonly Func<string, object?, object?, bool> _isEnabled;
        private IDisposable? _innerSubscription;

        public FilteringListener(string targetName, Action<string, object> onNext, Func<string, object?, object?, bool> isEnabled)
        {
            _targetName = targetName;
            _onNext = onNext;
            _isEnabled = isEnabled;
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == _targetName)
            {
                _innerSubscription = value.Subscribe(new AnonymousObserver(_onNext), _isEnabled);
            }
        }

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        public void Dispose()
        {
            _innerSubscription?.Dispose();
        }
    }

    private class AnonymousObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly Action<string, object> _onNext;

        public AnonymousObserver(Action<string, object> onNext)
        {
            _onNext = onNext;
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(KeyValuePair<string, object> value)
        {
            _onNext(value.Key, value.Value);
        }
    }
}
