using System;
using System.Diagnostics;
using DefaultEcs.System;

namespace CanopyEngine.Profiling;

/// <summary>
/// Wraps an ISystem<float> and reports execution time to SystemProfiler.
/// </summary>
public sealed class TimedSystem : ISystem<float>, IDisposable
{
    private readonly ISystem<float> _inner;
    private readonly string _name;
    private readonly Stopwatch _sw = new Stopwatch();
    private bool _enabled = true;

    public string Name => _name;

    public bool IsEnabled { get; set; } = true;

    public TimedSystem(ISystem<float> inner, string? name = null)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _name = name ?? inner.GetType().Name;
        SystemProfiler.Register(_name);
    }

    public void Update(float state)
    {
        if (!_enabled)
        {
            _inner.Update(state);
            return;
        }

        _sw.Restart();
        _inner.Update(state);
        _sw.Stop();

        SystemProfiler.ReportSample(_name, _sw.Elapsed.TotalMilliseconds);
    }

    public void Dispose()
    {
        SystemProfiler.Unregister(_name);
        (_inner as IDisposable)?.Dispose();
    }

    public void SetEnabled(bool enabled) => _enabled = enabled;
}
