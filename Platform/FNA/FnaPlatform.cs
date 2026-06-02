using CanopyEngine;
using Microsoft.Xna.Framework;
using Platform.Abstractions;

namespace Platform.FNA;

public sealed class FnaPlatform : Game, IPlatform
{
    private readonly GraphicsDeviceManager _gdm;
    private readonly FnaWindow _window;
    private readonly FnaInput _input;
    private readonly FnaTime _time;
    private FnaRenderer _renderer;

    private Engine _engine;

    public event Action? Initialized;

    public FnaPlatform(WindowConfig config)
    {
        //Window
        _window = new FnaWindow(Window, config);

        //Graphics
         _gdm = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = config.Width,
            PreferredBackBufferHeight = config.Height
        };
        _gdm.ApplyChanges();

        //Subsystems
        _input = new FnaInput();
        _time = new FnaTime();
    }

    //IPlatform
    public IWindow CreateWindow(WindowConfig config) => _window;
    public IInput Input => _input;
    public ITime Time => _time;

    public IRenderer Renderer => _renderer;

    protected override void Initialize()
    {
        base.Initialize();
        _renderer = new FnaRenderer(GraphicsDevice);
        Initialized?.Invoke();
    }

    protected override void Draw(GameTime gameTime)
    {
        _renderer.Clear(0f, 0f, 0f);
        _renderer.Begin();
        _engine?.Draw();
        _renderer.End();

        base.Draw(gameTime);
    }

    protected override void Update(GameTime gameTime)
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _time.Update(gameTime);
        _engine?.Update(delta);

        base.Update(gameTime);
    }

    public void SetEngine(Engine engine)
    {
        _engine = engine;
        engine.StopRequested += () => Exit();
    }
}