using CanopyEngine.Platform.Abstractions;
using Microsoft.Xna.Framework;

namespace CanopyEngine.Platform.FNA;

public sealed class FnaWindow : IWindow
{
    private readonly GameWindow _window;

    public FnaWindow (GameWindow window, WindowConfig config)
    {
        _window = window;
        _window.Title = config.Title;
    }

    public void PumpEvents()
    {
        //FNA handles this internally
    }

    public bool ShouldClose => false; //also handled internally.
}