using System.Numerics;

namespace Platform.Abstractions;

public interface IRenderer
{
    void Clear(float r, float g, float b);

    void DrawSprite(Sprite sprite, Vector2 position, float rotation, Color tint);
}