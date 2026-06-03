using Platform.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Numerics;
using System.Collections.Generic;

namespace Platform.FNA
{
    public sealed class FnaInput : IInput
    {
        private KeyboardState _prevKs;
        private KeyboardState _currKs;
        private MouseState _prevMs;
        private MouseState _currMs;

        private readonly List<KeyCode> _keysDown = new();
        private readonly List<KeyCode> _keysPressed = new();
        private readonly List<KeyCode> _keysReleased = new();

        private readonly List<MouseButton> _mbDown = new();
        private readonly List<MouseButton> _mbPressed = new();
        private readonly List<MouseButton> _mbReleased = new();

        public IReadOnlyList<KeyCode> KeysDown => _keysDown;
        public IReadOnlyList<KeyCode> KeysPressedThisFrame => _keysPressed;
        public IReadOnlyList<KeyCode> KeysReleasedThisFrame => _keysReleased;

        public IReadOnlyList<MouseButton> MouseButtonsDown => _mbDown;
        public IReadOnlyList<MouseButton> MouseButtonsPressedThisFrame => _mbPressed;
        public IReadOnlyList<MouseButton> MouseButtonsReleasedThisFrame => _mbReleased;

        public System.Numerics.Vector2 MousePosition => new(_currMs.X, _currMs.Y);

        public void Update()
        {
            // sample
            _prevKs = _currKs;
            _prevMs = _currMs;
            _currKs = Keyboard.GetState();
            _currMs = Mouse.GetState();

            // clear per-frame lists
            _keysDown.Clear();
            _keysPressed.Clear();
            _keysReleased.Clear();
            _mbDown.Clear();
            _mbPressed.Clear();
            _mbReleased.Clear();

            // populate key lists (iterate only the keys you care about for perf, or all Keys enum)
            foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
            {
                var mapped = MapKey(kc);
                if (mapped == Keys.None) continue;

                bool prev = _prevKs.IsKeyDown(mapped);
                bool curr = _currKs.IsKeyDown(mapped);

                if (curr) _keysDown.Add(kc);
                if (!prev && curr) _keysPressed.Add(kc);
                if (prev && !curr) _keysReleased.Add(kc);
            }

            // mouse buttons
            ProcessMouseButton(MouseButton.Left, _prevMs.LeftButton, _currMs.LeftButton);
            ProcessMouseButton(MouseButton.Right, _prevMs.RightButton, _currMs.RightButton);
            ProcessMouseButton(MouseButton.Middle, _prevMs.MiddleButton, _currMs.MiddleButton);
        }

        private void ProcessMouseButton(MouseButton b, ButtonState prev, ButtonState curr)
        {
            if (curr == ButtonState.Pressed) _mbDown.Add(b);
            if (prev == ButtonState.Released && curr == ButtonState.Pressed) _mbPressed.Add(b);
            if (prev == ButtonState.Pressed && curr == ButtonState.Released) _mbReleased.Add(b);
        }

        public bool IsKeyDown(KeyCode key) => _currKs.IsKeyDown(MapKey(key));
        public bool IsKeyPressed(KeyCode key) => _keysPressed.Contains(key);
        public bool IsKeyReleased(KeyCode key) => _keysReleased.Contains(key);

        public bool IsMouseDown(MouseButton button) => button switch
        {
            MouseButton.Left => _currMs.LeftButton == ButtonState.Pressed,
            MouseButton.Right => _currMs.RightButton == ButtonState.Pressed,
            MouseButton.Middle => _currMs.MiddleButton == ButtonState.Pressed,
            _ => false
        };

        public bool IsMousePressed(MouseButton button) => _mbPressed.Contains(button);
        public bool IsMouseReleased(MouseButton button) => _mbReleased.Contains(button);

        public float GetAxis(string axisName)
        {
            var ks = _currKs;
            float v = 0f;
            if (axisName == "Horizontal")
            {
                if (ks.IsKeyDown(Keys.A) || ks.IsKeyDown(Keys.Left)) v -= 1f;
                if (ks.IsKeyDown(Keys.D) || ks.IsKeyDown(Keys.Right)) v += 1f;
            }
            else if (axisName == "Vertical")
            {
                if (ks.IsKeyDown(Keys.W) || ks.IsKeyDown(Keys.Up)) v -= 1f;
                if (ks.IsKeyDown(Keys.S) || ks.IsKeyDown(Keys.Down)) v += 1f;
            }
            return MathHelper.Clamp(v, -1f, 1f);
        }

        // Map your KeyCode enum to MonoGame Keys
        private Keys MapKey(KeyCode key) => key switch
        {
            KeyCode.A => Keys.A,
            KeyCode.D => Keys.D,
            KeyCode.W => Keys.W,
            KeyCode.S => Keys.S,
            KeyCode.Left => Keys.Left,
            KeyCode.Right => Keys.Right,
            KeyCode.Up => Keys.Up,
            KeyCode.Down => Keys.Down,
            KeyCode.Escape => Keys.Escape,
            _ => Keys.None
        };
    }
}
