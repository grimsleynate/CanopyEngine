namespace Platform.Abstractions;

public interface IAssetManager
{
    ITexture LoadTexture(string path);
    Sprite LoadSprite(string path);
}