using Moq;
using Xunit;
using CanopyEngine.Platform.Abstractions;

namespace CanopyEngine.Tests
{
    public class EngineTests
    {
        private readonly float deltaTime = 0.016f; // ~60 FPS

        private Mock<IPlatform> CreateMockPlatform()
        {
            var mockPlatform = new Mock<IPlatform>();
            var mockInput = new Mock<IInput>();

            mockPlatform.Setup(p => p.Input).Returns(mockInput.Object);
            mockPlatform.Setup(p => p.Renderer).Returns((IRenderer)null);

            return mockPlatform;
        }

        [Fact]
        public void Update_WhenNotRunning_DoesNotInvokeCallbacks()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var updateInvoked = false;

            engine.OnUpdate += _ => updateInvoked = true;

            // Don't call Start()

            // Act
            engine.Update(deltaTime);

            // Assert
            Assert.False(updateInvoked);
        }

        [Fact]
        public void Update_WithValidDeltaTime_InvokesOnUpdateCallback()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var updateInvoked = false;
            float capturedDelta = 0f;

            engine.OnUpdate += delta =>
            {
                updateInvoked = true;
                capturedDelta = delta;
            };

            engine.Start();

            // Act
            engine.Update(deltaTime);

            // Assert
            Assert.True(updateInvoked);
            Assert.Equal(deltaTime, capturedDelta);
        }

        [Fact]
        public void Start_WiresInputFromPlatform()
        {
            // Arrange
            var mockPlatform = new Mock<IPlatform>();
            var mockInput = new Mock<IInput>();

            mockPlatform.Setup(p => p.Input).Returns(mockInput.Object);
            mockPlatform.Setup(p => p.Renderer).Returns((IRenderer)null);

            var engine = new Engine(mockPlatform.Object);

            // Act
            engine.Start();
            engine.Update(deltaTime);

            // Assert
            mockInput.Verify(i => i.Update(), Times.Once);
        }

        [Fact]
        public void Stop_RaisesStopRequestedEvent()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var stopRequested = false;

            engine.StopRequested += () => stopRequested = true;
            engine.Start();

            // Act
            engine.Stop();

            // Assert
            Assert.True(stopRequested);
        }

        [Fact]
        public void Stop_TransitionsToStopped()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);

            engine.Start();
            Assert.Equal(EngineState.Running, engine.State);

            // Act
            engine.Stop();

            // Assert
            Assert.Equal(EngineState.Stopped, engine.State);
        }

        [Fact]
        public void Stop_IsIdempotent()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var stopCount = 0;

            engine.StopRequested += () => stopCount++;
            engine.Start();

            // Act
            engine.Stop();
            engine.Stop();
            engine.Stop();

            // Assert - event should only fire once
            Assert.Equal(1, stopCount);
        }

        [Fact]
        public void Update_ClampsDeltaTimeToMaxDeltaSeconds()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object)
            {
                MaxDeltaSeconds = 0.05 // 50ms max
            };
            float capturedDelta = 0f;

            engine.OnUpdate += delta => capturedDelta = delta;
            engine.Start();

            // Act
            engine.Update(0.1f); // 100ms - should be clamped to 50ms

            // Assert
            Assert.Equal(0.05f, capturedDelta);
        }

        [Fact]
        public void Update_AccumulatesAndDispatchesFixedUpdates()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var fixedUpdateCount = 0;

            engine.OnFixedUpdate += _ => fixedUpdateCount++;
            engine.Start();

            // Act - send 3 frames that should accumulate to trigger fixed update
            engine.Update(0.01f);
            engine.Update(0.01f);
            engine.Update(0.01f);

            // Assert - at least one fixed update should have fired
            Assert.True(fixedUpdateCount > 0);
        }

        [Fact]
        public void FixedUpdate_InvokesOnFixedUpdateCallback()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var fixedUpdateInvoked = false;
            float capturedFixedDelta = 0f;

            engine.OnFixedUpdate += delta =>
            {
                fixedUpdateInvoked = true;
                capturedFixedDelta = delta;
            };

            engine.Start();

            // Act
            var fixedDelta = (float)(1.0 / 60.0);
            engine.FixedUpdate(fixedDelta);

            // Assert
            Assert.True(fixedUpdateInvoked);
            Assert.Equal(fixedDelta, capturedFixedDelta, 5);
        }

        [Fact]
        public void Draw_InvokesOnDrawCallback()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var drawInvoked = false;

            engine.OnDraw += () => drawInvoked = true;
            engine.Start();

            // Act
            engine.Draw();

            // Assert
            Assert.True(drawInvoked);
        }

        [Fact]
        public void Draw_WhenNotRunning_DoesNotInvokeCallback()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var drawInvoked = false;

            engine.OnDraw += () => drawInvoked = true;

            // Don't call Start()

            // Act
            engine.Draw();

            // Assert
            Assert.False(drawInvoked);
        }
    }
}