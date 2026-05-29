using Moq;
using Xunit;
using CanopyEngine.Platform.Abstractions;

namespace CanopyEngine.Tests
{
    public class EngineTests
    {
        [Fact]
        public void Update_WhenRendererNull_DoesNotThrow()
        {
            // Arrange
            var mockPlatform = new Mock<IPlatform>();
            var mockInput = new Mock<IInput>();
            var engine = new Engine(mockPlatform.Object);

            mockPlatform.Setup(p => p.Input).Returns(mockInput.Object);
            mockPlatform.Setup(p => p.Renderer).Returns((IRenderer)null); // Explicitly null

            // Act & Assert
            engine.Update();
        }

        [Fact]
        public void Update_WhenRendererReady_CallsClearWithCorrectColor()
        {
            // Arrange
            var mockPlatform = new Mock<IPlatform>();
            var mockInput = new Mock<IInput>();
            var mockRenderer = new Mock<IRenderer>();
            var engine = new Engine(mockPlatform.Object);

            mockPlatform.Setup(p => p.Input).Returns(mockInput.Object);
            mockPlatform.Setup(p => p.Renderer).Returns(mockRenderer.Object);
            mockInput.Setup(i => i.IsKeyDown(KeyCode.Space)).Returns(false);

            // Act
            engine.Update();

            // Assert
            mockRenderer.Verify(r => r.Clear(0.1f, 0.1f, 0.1f), Times.Once);
        }
    }
}