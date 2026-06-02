namespace Platform.Abstractions;

public interface IPlatform
{
    IWindow CreateWindow(WindowConfig config);
    IInput Input { get; }
    ITime Time { get;}
    IRenderer Renderer { get; }
    event Action? Initialized;
}