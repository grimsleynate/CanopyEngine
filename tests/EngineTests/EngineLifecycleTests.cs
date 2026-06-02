using CanopyEngine;
using Moq;
using Platform.Abstractions;

namespace Tests
{
    public class EngineLifecycleTests
    {
        private Mock<IPlatform> CreateMockPlatform()
        {
            var mockPlatform = new Mock<IPlatform>();
            var mockInput = new Mock<IInput>();

            mockPlatform.Setup(p => p.Input).Returns(mockInput.Object);
            mockPlatform.Setup(p => p.Renderer).Returns((IRenderer)null);

            return mockPlatform;
        }

        #region Starting / Stopped State

        [Fact]
        public void Engine_StartsInStoppedState()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);

            // Assert
            Assert.Equal(EngineState.Stopped, engine.State);
            Assert.False(engine.IsRunning);
        }

        [Fact]
        public void Start_TransitionsToRunning()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);

            // Act
            engine.Start();

            // Assert
            Assert.Equal(EngineState.Running, engine.State);
            Assert.True(engine.IsRunning);
        }

        [Fact]
        public void Start_IsIdempotent()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);

            // Act
            engine.Start();
            var stateAfterFirstStart = engine.State;
            engine.Start();
            var stateAfterSecondStart = engine.State;

            // Assert
            Assert.Equal(EngineState.Running, stateAfterFirstStart);
            Assert.Equal(EngineState.Running, stateAfterSecondStart);
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
            engine.Update(0.016f); // 60 FPS frame time

            // Assert
            mockInput.Verify(i => i.Update(), Times.Once);
        }

        #endregion

        #region Update with DeltaTime

        [Fact]
        public void Update_WithValidDeltaTime_InvokesOnUpdate()
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
            engine.Update(0.016f); // 60 FPS

            // Assert
            Assert.True(updateInvoked);
            Assert.Equal(0.016f, capturedDelta);
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
            engine.Update(0.1f); // 100ms - should be clamped

            // Assert
            Assert.Equal(0.05f, capturedDelta);
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
            engine.Update(0.016f);

            // Assert
            Assert.False(updateInvoked);
        }

        #endregion

        #region FixedUpdate

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
            var fixedDelta = (float)(1.0 / 60.0); // Default fixed timestep
            engine.FixedUpdate(fixedDelta);

            // Assert
            Assert.True(fixedUpdateInvoked);
            Assert.Equal(fixedDelta, capturedFixedDelta, 5);
        }

        [Fact]
        public void FixedUpdate_WhenNotRunning_DoesNotInvokeCallback()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var fixedUpdateInvoked = false;

            engine.OnFixedUpdate += _ => fixedUpdateInvoked = true;

            // Don't call Start()

            // Act
            engine.FixedUpdate(1.0f / 60.0f);

            // Assert
            Assert.False(fixedUpdateInvoked);
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

            // Act - send 3 frames of 16.67ms (should trigger 1 fixed update at 16.67ms)
            engine.Update(0.01667f); // One fixed step should trigger
            engine.Update(0.01667f);
            engine.Update(0.01667f);

            // Assert
            Assert.True(fixedUpdateCount > 0);
        }

        #endregion

        #region Draw

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

        #endregion

        #region Stopping / Stopped

        [Fact]
        public void Stop_TransitionsToStopped()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            engine.Start();

            // Act
            engine.Stop();

            // Assert
            Assert.Equal(EngineState.Stopped, engine.State);
            Assert.False(engine.IsRunning);
        }

        [Fact]
        public void Stop_RaisesStopRequestedEvent()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var stopRequestedFired = false;

            engine.StopRequested += () => stopRequestedFired = true;
            engine.Start();

            // Act
            engine.Stop();

            // Assert
            Assert.True(stopRequestedFired);
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
        public void Update_AfterStop_DoesNotInvokeCallbacks()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            var updateInvoked = false;

            engine.OnUpdate += _ => updateInvoked = true;
            engine.Start();
            engine.Stop();

            // Act
            engine.Update(0.016f);

            // Assert
            Assert.False(updateInvoked);
        }

        #endregion

        #region Exception Handling

        [Fact]
        public void Update_WhenOnUpdateThrows_WrapsInInvalidOperationException()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);

            engine.OnUpdate += _ => throw new ArgumentException("Test error");
            engine.Start();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => engine.Update(0.016f));
            Assert.Contains("Unhandled exception in OnUpdate handler", ex.Message);
        }

        [Fact]
        public void FixedUpdate_WhenOnFixedUpdateThrows_WrapsInInvalidOperationException()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);

            engine.OnFixedUpdate += _ => throw new ArgumentException("Test error");
            engine.Start();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => engine.FixedUpdate(1.0f / 60.0f));
            Assert.Contains("Unhandled exception in OnFixedUpdate handler", ex.Message);
        }

        [Fact]
        public void Draw_WhenOnDrawThrows_WrapsInInvalidOperationException()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);

            engine.OnDraw += () => throw new ArgumentException("Test error");
            engine.Start();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => engine.Draw());
            Assert.Contains("Unhandled exception in OnDraw handler", ex.Message);
        }

        #endregion

        #region Cancellation Token

        [Fact]
        public void GetCancellationToken_WhenRunning_ReturnsValidToken()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            engine.Start();

            // Act
            var token = engine.GetCancellationToken();

            // Assert
            Assert.False(token.IsCancellationRequested);
        }

        [Fact]
        public void GetCancellationToken_AfterStop_IsRequestedCancellation()
        {
            // Arrange
            var mockPlatform = CreateMockPlatform();
            var engine = new Engine(mockPlatform.Object);
            engine.Start();
            var token = engine.GetCancellationToken();

            // Act
            engine.Stop();

            // Assert
            Assert.True(token.IsCancellationRequested);
        }

        #endregion
    }
}