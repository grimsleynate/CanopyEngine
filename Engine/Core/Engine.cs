using CanopyEngine.Platform.Abstractions;

namespace CanopyEngine;

public class Engine
{
    private readonly IPlatform _platform;
    private readonly IInput _input;

    public Engine(IPlatform platform)
    {
        _platform = platform;
        _input = _platform.Input;
    }

    public void Update()
    {
        _input.Update();

        if (_input.IsKeyDown(KeyCode.W))
            Console.WriteLine("W is down.");

        if (_input.IsMouseDown(MouseButton.Left))
            Console.WriteLine("Left Mouse Button clicked");

        var (mx, my) = _input.MousePosition;
        Console.WriteLine($"Mouse:{mx}, {my}");
    }
}