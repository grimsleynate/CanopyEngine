namespace Platform.Abstractions;

public interface IWindow
{
    void PumpEvents();
    bool ShouldClose { get; }
}