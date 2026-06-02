using Microsoft.Xna.Framework;
using Platform.Abstractions;

namespace Platform.FNA;

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