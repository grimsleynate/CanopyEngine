using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Platform.Abstractions;
using System.Reflection.Metadata;

namespace Platform.FNA;

public sealed class FnaRenderer : IRenderer
{
    private readonly GraphicsDevice _gd;
    private readonly SpriteBatch _spriteBatch;

    private readonly Texture2D _whitePixel;

    public FnaRenderer(GraphicsDevice gd)
    {
        _gd = gd;
        _spriteBatch = new SpriteBatch(gd);

        _whitePixel = new Texture2D(gd, 1, 1);
        _whitePixel.SetData(new[] { Microsoft.Xna.Framework.Color.White });
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

    public void DrawQuad(System.Numerics.Vector2 position, float width, float height, uint color)
    {
        byte a = (byte)((color >> 24) & 0xFF);
        byte r = (byte)((color >> 16) & 0xFF);
        byte g = (byte)((color >> 8) & 0xFF);
        byte b = (byte)(color & 0xFF);
        var newColor = new Microsoft.Xna.Framework.Color(r, g, b, a);

        var dest = new Rectangle(
        (int)position.X, (int)position.Y, (int)width, (int)height);
        _spriteBatch.Draw(_whitePixel, dest, newColor);
    }

    public void DrawString(SpriteFont font, string text, Vector2 position, Microsoft.Xna.Framework.Color color)
    {
        _spriteBatch.DrawString(font, text, position, color);
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