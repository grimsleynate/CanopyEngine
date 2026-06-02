using Platform.Abstractions;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Platform.FNA;

public sealed class FnaRenderer : IRenderer
{
    private readonly GraphicsDevice _gd;
    private readonly SpriteBatch _spriteBatch;

    public FnaRenderer(GraphicsDevice gd)
    {
        _gd = gd;
        _spriteBatch = new SpriteBatch(gd);
    }

    public void Begin()
    {
        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.NonPremultiplied,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone
        );
    }

    public void Clear(float r, float g, float b)
    {
        _gd.Clear(new Microsoft.Xna.Framework.Color(
            (byte)(r * 255),
            (byte)(g * 255),
            (byte)(b * 255),
            255
        ));
    }

    public void DrawSprite(Sprite sprite, System.Numerics.Vector2 pos, float rot, Abstractions.Color tint)
    {
        var fnaTex = sprite.Texture as FnaTexture;
        _spriteBatch.Draw(
            fnaTex?.Texture,
            new Vector2(pos.X, pos.Y),
            null,
            ToXnaColor(tint),
            rot,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0f
        );
    }

    public void End()
    {
        _spriteBatch.End();
    }

    private static Microsoft.Xna.Framework.Color ToXnaColor(Abstractions.Color c)
    {
        return new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);
    }
}