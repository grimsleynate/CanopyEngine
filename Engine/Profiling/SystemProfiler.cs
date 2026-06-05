using System.Collections.Concurrent;

namespace CanopyEngine.Profiling;

public sealed class SystemStats
{
    public string Name { get; init; } = string.Empty;
    public double LastMs { get; set; }
    public double EmaMs { get; set; }
    public long Samples { get; set; }
}

public static class SystemProfiler
{
    private static readonly ConcurrentDictionary<string, SystemStats> _stats = new();
    private static double _alpha = 1.0 / 60.0; // EMA alpha (60-frame window)

    public static IReadOnlyCollection<SystemStats> Stats => (IReadOnlyCollection<SystemStats>)_stats.Values;

    public static void SetEmaAlpha(double alpha) => _alpha = alpha;

    public static void Register(string name)
    {
        _stats.TryAdd(name, new SystemStats { Name = name });
    }

    public static void Unregister(string name)
    {
        _stats.TryRemove(name, out _);
    }

    public static void ReportSample(string name, double ms)
    {
        var stats = _stats.GetOrAdd(name, _ => new SystemStats { Name = name });
        stats.LastMs = ms;
        if (stats.Samples == 0)
            stats.EmaMs = ms;
        else
            stats.EmaMs = stats.EmaMs * (1.0 - _alpha) + ms * _alpha;
        stats.Samples++;
    }

    public static void Reset() => _stats.Clear();
}
