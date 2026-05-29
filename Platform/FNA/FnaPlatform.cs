using Microsoft.Xna.Framework;
using CanopyEngine.Platform.Abstractions;

namespace CanopyEngine.Platform.FNA;

public sealed class FnaPlatform : Game, IPlatform
{
    private readonly GraphicsDeviceManager _gdm;
    private readonly FnaWindow _window;
    private readonly FnaInput _input;
    private readonly FnaTime _time;

    private Engine _engine;

    public FnaPlatform(WindowConfig config)
    {
        //Window
        _window = new FnaWindow(Window, config);

        //Graphics
        var gdm = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = config.Width,
            PreferredBackBufferHeight = config.Height
        };
        gdm.ApplyChanges();

        //Subsystems
        _input = new FnaInput();
        _time = new FnaTime();
    }

    //IPlatform
    public IWindow CreateWindow(WindowConfig config) => _window;
    public IInput Input => _input;
    public ITime Time => _time;

    protected override void Update(GameTime gameTime)
    {
        _time.Update(gameTime);
        _input.Update();
        _engine.Update();

        base.Update(gameTime);
    }

    public void SetEngine(Engine engine)
    {
        _engine = engine;
    }
}