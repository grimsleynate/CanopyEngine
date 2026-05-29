using CanopyEngine.Platform.Abstractions;

namespace CanopyEngine;

public class Engine
{
    private readonly IPlatform _platform;
    private readonly IInput _input;
    private IRenderer _renderer;

    public Engine(IPlatform platform)
    {
        _platform = platform;
        _input = _platform.Input;
    }

    public void Update()
    {
        _input.Update();

        if (_renderer == null)
        {
            _renderer = _platform.Renderer;
        }

        var color = _input.IsKeyDown(KeyCode.Space)
        ? (0f, 0.2f, 0.6f)
        : (0.1f, 0.1f, 0.1f);

        _renderer.Clear(color.Item1, color.Item2, color.Item3);
    }
}