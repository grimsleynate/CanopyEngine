using Microsoft.Xna.Framework.Graphics;
using Platform.Abstractions;
using System.Drawing;

namespace Platform.FNA
{
    public sealed class FnaAssetManager : IAssetManager
    {
        private readonly GraphicsDevice _graphics;

        public FnaAssetManager(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        public ITexture LoadTexture(string path)
        {
            using var stream = File.OpenRead(path);
            var tex = Texture2D.FromStream(_graphics, stream);
            return new FnaTexture(tex);
        }

        public Sprite LoadSprite(string path)
        {
            var tex = LoadTexture(path);
            return new Sprite
            {
                Texture = tex,
                UV = new RectangleF(0, 0, 1, 1),
                Width = tex.Width,
                Height = tex.Height
            };
        }
    }
}
