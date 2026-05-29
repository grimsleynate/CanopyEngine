using Microsoft.Xna.Framework;
using CanopyEngine.Platform.Abstractions;

namespace CanopyEngine.Platform.FNA;

public sealed class FnaPlatform : Game, IPlatform
{
    private readonly GraphicsDeviceManager _gdm;
    private readonly FnaWindow _window;
    private readonly FnaInput _input;
    private readonly FnaTime _time;
    private FnaRenderer _renderer;

    private Engine _engine;

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
    }

    protected override void Draw(GameTime gameTime)
    {
        _engine?.Draw();
        base.Draw(gameTime);
    }

    protected override void Update(GameTime gameTime)
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _time.Update(gameTime);
        _engine.Update(delta);

        base.Update(gameTime);
    }

    public void SetEngine(Engine engine)
    {
        _engine = engine;
        //WHen the engine requests stop, exit the game loop
        engine.StopRequested += () => Exit();
    }
}