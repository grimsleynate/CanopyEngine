using System.Drawing;

namespace Platform.Abstractions;

public struct Sprite
{
    public ITexture Texture;
    public RectangleF UV;
    public int Width;
    public int Height;

    public bool IsValid => Texture != null;

    public Sprite(ITexture texture, RectangleF uv, int width, int height)
    {
        Texture = texture;
        UV = uv;
        Width = width;
        Height = height;
    }
}