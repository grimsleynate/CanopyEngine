using Microsoft.Xna.Framework;
using CanopyEngine.Platform.Abstractions;

public sealed class FnaTime: ITime
{
    public double TotalTime { get; private set; }
    public double DeltaTime { get; private set; }

    internal void Update(GameTime gameTime)
    {
        DeltaTime = gameTime.ElapsedGameTime.TotalSeconds;
        TotalTime = gameTime.TotalGameTime.TotalSeconds;
    }
}