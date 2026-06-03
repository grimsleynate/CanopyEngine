using DefaultEcs;
using DefaultEcs.System;
using Platform.Abstractions;

namespace CanopyEngine;

/// <summary>
/// Core engine class.
/// The engine itself is platform-agnostic; the platform should call
/// SetEngine(thisEngine) and subscribe to StopRequested to exit the native loop.
/// </summary>
public class Engine
{
    private readonly IPlatform _platform;
    private IInput _input;
    private IRenderer _renderer;

    private ISystem<float> _updateSystems;
    private ISystem<float> _fixedSystems;
    private ISystem<float> _drawSystems;

    private readonly ITime _time;
    public uint FrameCount { get; private set; }
    public uint FixedUpdateCount { get; private set; }
    public ITime Time => _time;

    private CancellationTokenSource? _cts;
    private readonly object _stateLock = new();

    //Fixed-step accumulator
    private double _accumulatorSeconds = 0.0;

    /// <summary>Maximum delta seconds allowed per frame</summary>
    public double MaxDeltaSeconds {get; set;} = 0.25; // 250 ms

    /// <summary>Fixed step size in seconds (default 60 Hz).</summary>
    public double FixedDeltaSeconds { get; set; } = 1.0 / 60.0;

    /// <summary>The main ECS world. This is created by the engine and can be used by systems and game code. </summary>
    public World World { get; }

    /// <summary>
    /// Current lifecylce state of the engine
    /// </summary>
    public EngineState State { get; private set; } = EngineState.Stopped;

    /// <summary>
    /// Raised when the engine requests the host/platform to stop (e.g., user requested quit).
    /// Platform should subscribe and call its Exit/Stop method.
    /// </summary>
    public event Action? StopRequested;

    /// <summary>
    /// Event invoked once when the engine starts. This is useful for initialization that needs to happen after Start() is called.</summary>
    public event Action? OnStart;

    /// <summary>Event invoked every variable update (deltaSeconds).</summary>
    public event Action<float>? OnUpdate;

    /// <summary>Event invoked for each fixed step (fixedDeltaSeconds).</summary>
    public event Action<float>? OnFixedUpdate;

    /// <summary>Event invoked once per frame for draw submission.</summary>
    public event Action? OnDraw;

    public Engine(IPlatform platform)
    {
        _platform = platform;
        World = new World();
        _time = platform.Time;
    }

    /// <summary>
    /// Start the engine. This transitions the engine into Running state.
    /// This method is idempotent: calling Start when already running is a no-op.
    /// </summary>
    public void Start()
    {
        lock (_stateLock)
        {
            if (State == EngineState.Running || State == EngineState.Starting)
                return;

            State = EngineState.Starting;
            _cts = new CancellationTokenSource();
            _accumulatorSeconds = 0.0;

            //wire in subsystems
            _input = _platform.Input;

            State = EngineState.Running;
            OnStart?.Invoke();
        }
    }

    /// <summary>
    /// Request the engine to stop. This raises StopRequested so the platform can exit its loop.
    /// Stop is idempotent.
    /// </summary>
    public void Stop()
    {
        lock ( _stateLock)
        {
            if (State == EngineState.Stopped || State == EngineState.Stopping)
                return;

            State = EngineState.Stopping;

            //signal any running tasks
            try { _cts?.Cancel(); } catch { }

            // Notify the platform to exit its loop.
            StopRequested?.Invoke();

            //Finalize state
            State = EngineState.Stopped;
        }
    }

    /// <summary>
    /// Called by the platform each frame. This method is intentionally small and safe:
    /// - If the engine is not running, Update is a no-op.
    /// - If renderer is null, we skip rendering calls (useful for unit tests).
    /// </summary>
    public void Update(float deltaSeconds)
    {
        if (!IsRunning)
            return;

        var clamped = (float)Math.Min(deltaSeconds, MaxDeltaSeconds);

        _input.Update();

        try
        {
            OnUpdate?.Invoke(clamped);
            _updateSystems.Update(clamped);
            FrameCount++;
            Console.WriteLine($"FrameCount: {FrameCount}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unhandled exception in OnUpdate handler.", ex);
        }

        _accumulatorSeconds += clamped;

        var maxAccum = FixedDeltaSeconds * 5.0;
        if (_accumulatorSeconds > maxAccum)
            _accumulatorSeconds = maxAccum;

        while (_accumulatorSeconds >= FixedDeltaSeconds)
        {
            try
            {
                OnFixedUpdate?.Invoke((float)FixedDeltaSeconds);
                _fixedSystems.Update((float)FixedDeltaSeconds);
                FixedUpdateCount++;
                Console.WriteLine($"FixedUpdateCount: {FixedUpdateCount}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unhandled exception in OnUpdate handler.", ex);
            }

            _accumulatorSeconds -= FixedDeltaSeconds;

            Console.WriteLine($"Total Time: {Time.TotalTime} Delta Time: {Time.DeltaTime} Unscaled Delta Time: {Time.UnscaledDeltaTime} Time Scale: {Time.TimeScale} ");
        }
    }

    /// <summary>
    /// Deterministic fixed-step update.
    /// </summary>
    public void FixedUpdate(float fixedDeltaSeconds)
    {
        if (!IsRunning)
            return;

        try
        {
            OnFixedUpdate?.Invoke(fixedDeltaSeconds);

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unhandled exception in OnFixedUpdate handler.", ex);
        }
    }

    /// <summary>
    /// Called once per frame to submit rendering commands. 
    /// Draw is for final submission and optional engine-level overlays.
    /// Platform should call Draw() from its Draw() override and then present.
    /// </summary>
    public void Draw()
    {
        if (!IsRunning)
            return;

        try
        {
            _drawSystems?.Update(0f);
            OnDraw?.Invoke();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unhandled exception in OnDraw handler.", ex);
        }
    }

    /// <summary>
    /// Whether the engine is currently running.
    /// </summary>
    public bool IsRunning
    {
        get
        {
            lock (_stateLock) { return State == EngineState.Running; }
        }
    }

    /// <summary>
    /// Returns a token that will be cancelled when Stop() is called.
    /// </summary>
    public CancellationToken GetCancellationToken()
    {
        return _cts?.Token ?? new CancellationToken(true);
    }

    public void SetUpdateSystems(ISystem<float> systems)
    {
        _updateSystems = systems;
    }

    public void SetFixedSystems(ISystem<float> systems)
    {
        _fixedSystems = systems;
    }

    public void SetDrawSystems(ISystem<float> systems)
    {
        _drawSystems = systems;
    }
}

/// <summary>
/// Simple lifecycle enum for the engine.
/// </summary>
public enum EngineState
{
    Stopped,
    Starting,
    Running,
    Stopping
}