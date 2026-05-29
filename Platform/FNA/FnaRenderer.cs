using CanopyEngine.Platform.Abstractions;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CanopyEngine.Platform.FNA;

public sealed class FnaRenderer : IRenderer
{
    private readonly GraphicsDevice _gd;
    public FnaRenderer(GraphicsDevice gd)
    {
        _gd = gd;
    }
    public void Clear(float r, float g, float b)
    {
        _gd.Clear(new Color(r, g, b));
    }
}