using Platform.Abstractions;
using Xunit;

namespace Tests
{
    public class SpriteStructTests
    {
        private sealed class FakeTexture : ITexture
        {
            public int Width { get; }

            public int Height { get; }
            public FakeTexture(int w, int h) { Width = w; Height = h; }
        }

        [Fact]
        public void Sprite_Constructor_SetsFields()
        {
            //Arrange
            var tex = new FakeTexture(160, 160);
            var uv = new System.Drawing.RectangleF(0f, 0f, 1f, 1f);

            //Act
            var sprite = new Sprite(tex, uv, tex.Width, tex.Height);

            //Assert
            Assert.Equal(tex, sprite.Texture);
            Assert.Equal(uv, sprite.UV);
            Assert.Equal(160, sprite.Width);
            Assert.Equal(160, sprite.Height);
        }
    }
}
