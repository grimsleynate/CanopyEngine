using Microsoft.Xna.Framework;
using Platform.Abstractions;

namespace Platform.FNA;

public sealed class FnaTime: ITime
{
    private double _unscaledDeltaTime;
    private double _timeScale = 1.0;
    private uint _frameCount = 0;
    private uint _fixedUpdateCount = 0;

    public double TotalTime { get; private set; }
    public double DeltaTime { get; private set; }
    public double UnscaledDeltaTime
    {
        get => _unscaledDeltaTime;
    }

    public double TimeScale
    {
        get => _timeScale;
        set => _timeScale = Math.Max(0.0, value);
    }

    public uint FrameCount => _frameCount;
    public uint FixedUpdateCount => _fixedUpdateCount;

    internal void Update(GameTime gameTime)
    {
        DeltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        TotalTime = gameTime.TotalGameTime.TotalSeconds;
    }

    internal void RecordFixedUpdate()
    {
        _fixedUpdateCount++;
    }
}