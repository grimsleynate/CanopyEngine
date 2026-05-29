using CanopyEngine.Platform.Abstractions;
using System;
using System.Threading;

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

    private CancellationTokenSource? _cts;
    private readonly object _stateLock = new();

    //Fixed-step accumulator
    private double _accumulatorSeconds = 0.0;

    /// <summary>Maximum delta seconds allowed per frame</summary>
    public double MaxDeltaSeconds {get; set;} = 0.25; // 250 ms

    /// <summary>Fixed step size in seconds (default 60 Hz).</summary>
    public double FixedDeltaSeconds { get; set; } = 1.0 / 60.0;

    /// <summary>
    /// Current lifecylce state of the engine
    /// </summary>
    public EngineState State { get; private set; } = EngineState.Stopped;

    /// <summary>
    /// Raised when the engine requests the host/platform to stop (e.g., user requested quit).
    /// Platform should subscribe and call its Exit/Stop method.
    /// </summary>
    public event Action? StopRequested;

    /// <summary>Event invoked every variable update (deltaSeconds).</summary>
    public event Action<float>? OnUpdate;

    /// <summary>Event invoked for each fixed step (fixedDeltaSeconds).</summary>
    public event Action<float>? OnFixedUpdate;

    /// <summary>Event invoked once per frame for draw submission.</summary>
    public event Action? OnDraw;

    public Engine(IPlatform platform)
    {
        _platform = platform;
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

        // Handle input
        if (_input.IsKeyDown(KeyCode.Escape))
        {
            Stop();
            return;
        }

        var bgColor = _input.IsKeyDown(KeyCode.Space)
            ? (0.0f, 0.2f, 0.6f) 
            : (0.1f, 0.1f, 0.1f);

        // Clear with the selected color
        _platform.Renderer?.Clear(bgColor.Item1, bgColor.Item2, bgColor.Item3);


        try
        {
            OnUpdate?.Invoke(clamped);
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
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unhandled exception in OnUpdate handler.", ex);
            }

            _accumulatorSeconds -= FixedDeltaSeconds;
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