namespace Platform.Abstractions;

public interface ITime
{
    double TotalTime { get; }
    double DeltaTime { get; }
}