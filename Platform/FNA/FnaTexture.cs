using Microsoft.Xna.Framework.Graphics;
using Platform.Abstractions;

namespace Platform.FNA;

public sealed class FnaTexture : ITexture
{
    public Texture2D Texture { get; }
    public int Width => Texture.Width;
    public int Height => Texture.Height;

    public FnaTexture(Texture2D texture)
    {
        Texture = texture;
    }
}
