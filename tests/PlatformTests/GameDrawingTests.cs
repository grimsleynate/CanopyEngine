using Moq;
using Platform.Abstractions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Tests;

public class GameDrawingTests
{
    private sealed class FakeTexture : ITexture
    {
        public int Width { get; }
        public int Height { get; }
        public FakeTexture(int w, int h) { Width = w; Height = h; }
    }

    [Fact]
    public void EngineDraw_WhenOnDrawIssuesDrawSprite_CallsRendererDrawSprite()
    {
        //Arrange
        var mockPlatform = new Mock<IPlatform>();
        var mockInput = new Mock<IInput>();
        var mockRenderer = new Mock<IRenderer>();

        mockPlatform.Setup(p => p.Input).Returns(mockInput.Object);
        mockPlatform.Setup(p => p.Renderer).Returns(mockRenderer.Object);

        var engine = new CanopyEngine.Engine(mockPlatform.Object);

        var sprite = new Sprite(new FakeTexture(160, 160), new System.Drawing.RectangleF(0, 0, 1, 1), 160, 160);
        var pos = new Vector2(100f, 100f);

        engine.OnDraw += () =>
        {
            mockPlatform.Object.Renderer.DrawSprite(sprite, pos, 0f, Color.White);
        };

        engine.Start();

        //Act
        engine.Draw();

        //Assert
        mockRenderer.Verify(r => r.DrawSprite(
                It.Is<Sprite>(s => s.Width == 160 && s.Height == 160),
                It.Is<Vector2>(v => v.X == 100f && v.Y == 100f),
                0f,
                It.IsAny<Platform.Abstractions.Color>()), Times.Once);
    }
}
