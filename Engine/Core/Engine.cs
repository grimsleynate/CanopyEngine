using CanopyEngine.Platform.Abstractions;

namespace CanopyEngine;

public class Engine
{
    private readonly IPlatform _platform;
    private IInput _input;
    private IRenderer _renderer;

    public Engine(IPlatform platform)
    {
        _platform = platform;
    }

    public void Update()
    {
        if (_input == null)
        {
            _input = _platform.Input;
        }

        _input.Update();

        if (_renderer == null)
        {
            _renderer = _platform.Renderer;
        }

        if (_renderer == null)
        {
            // Renderer isn't ready yet; nothing to draw. Fail-safe for tests.
            return;
        }

        var color = _input.IsKeyDown(KeyCode.Space)
        ? (0f, 0.2f, 0.6f)
        : (0.1f, 0.1f, 0.1f);

        _renderer.Clear(color.Item1, color.Item2, color.Item3);
    }
}