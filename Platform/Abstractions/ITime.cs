namespace Platform.Abstractions;

public interface ITime
{
    double TotalTime { get; }
    double DeltaTime { get; }
    double UnscaledDeltaTime { get; }
    double TimeScale { get; }
    uint FrameCount { get; }
    uint FixedUpdateCount { get; }
}