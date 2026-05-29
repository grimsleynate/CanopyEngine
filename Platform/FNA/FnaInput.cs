using CanopyEngine.Platform.Abstractions;
using Microsoft.Xna.Framework.Input;

namespace CanopyEngine.Platform.FNA;

internal class FnaInput : IInput
{
    private KeyboardState _prevKeyboard;
    private KeyboardState _currKeyboard;

    private MouseState _prevMouse;
    private MouseState _currMouse;

    public (int X, int Y) MousePosition => (_currMouse.X, _currMouse.Y);

    public IReadOnlyList<KeyCode> KeysDown => _currKeyboard.GetPressedKeys()
        .Select(FromFnaKey)
        .ToList();

    public IReadOnlyList<KeyCode> KeysPressedThisFrame => _currKeyboard.GetPressedKeys()
        .Except(_prevKeyboard.GetPressedKeys())
        .Select(FromFnaKey)
        .ToList();

    public IReadOnlyList<KeyCode> KeysReleasedThisFrame => _prevKeyboard.GetPressedKeys()
        .Except(_currKeyboard.GetPressedKeys())
        .Select(FromFnaKey)
        .ToList();

    public IReadOnlyList<MouseButton> MouseButtonsDown => Enum.GetValues<MouseButton>()
        .Where(w => IsButtonDown(_currMouse, w))
        .ToList();

    public IReadOnlyList<MouseButton> MouseButtonsPressedThisFrame => Enum.GetValues<MouseButton>()
        .Where(w => 
            !IsButtonDown(_prevMouse, w) &&
            IsButtonDown(_currMouse, w))
        .ToList();

    public IReadOnlyList<MouseButton> MouseButtonsReleasedThisFrame => Enum.GetValues<MouseButton>()
        .Where(w =>
            IsButtonDown(_prevMouse, w) &&
            !IsButtonDown(_currMouse, w))
        .ToList();

    public bool IsKeyDown(KeyCode key) => _currKeyboard.IsKeyDown(ToFnaKey(key));

    public bool IsMouseDown(MouseButton button) => IsButtonDown(_currMouse, button);

    private static Keys ToFnaKey(KeyCode key)
    {
        return key switch
        {
            KeyCode.None => Keys.None,
            KeyCode.Back => Keys.Back,
            KeyCode.Tab => Keys.Tab,
            KeyCode.Enter => Keys.Enter,
            KeyCode.Pause => Keys.Pause,
            KeyCode.CapsLock => Keys.CapsLock,
            KeyCode.Escape => Keys.Escape,
            KeyCode.Space => Keys.Space,
            KeyCode.PageUp => Keys.PageUp,
            KeyCode.PageDown => Keys.PageDown,
            KeyCode.End => Keys.End,
            KeyCode.Home => Keys.Home,
            KeyCode.Left => Keys.Left,
            KeyCode.Up => Keys.Up,
            KeyCode.Right => Keys.Right,
            KeyCode.Down => Keys.Down,
            KeyCode.Select => Keys.Select,
            KeyCode.Print => Keys.Print,
            KeyCode.Execute => Keys.Execute,
            KeyCode.PrintScreen => Keys.PrintScreen,
            KeyCode.Insert => Keys.Insert,
            KeyCode.Delete => Keys.Delete,
            KeyCode.Help => Keys.Help,

            KeyCode.D0 => Keys.D0,
            KeyCode.D1 => Keys.D1,
            KeyCode.D2 => Keys.D2,
            KeyCode.D3 => Keys.D3,
            KeyCode.D4 => Keys.D4,
            KeyCode.D5 => Keys.D5,
            KeyCode.D6 => Keys.D6,
            KeyCode.D7 => Keys.D7,
            KeyCode.D8 => Keys.D8,
            KeyCode.D9 => Keys.D9,

            KeyCode.A => Keys.A,
            KeyCode.B => Keys.B,
            KeyCode.C => Keys.C,
            KeyCode.D => Keys.D,
            KeyCode.E => Keys.E,
            KeyCode.F => Keys.F,
            KeyCode.G => Keys.G,
            KeyCode.H => Keys.H,
            KeyCode.I => Keys.I,
            KeyCode.J => Keys.J,
            KeyCode.K => Keys.K,
            KeyCode.L => Keys.L,
            KeyCode.M => Keys.M,
            KeyCode.N => Keys.N,
            KeyCode.O => Keys.O,
            KeyCode.P => Keys.P,
            KeyCode.Q => Keys.Q,
            KeyCode.R => Keys.R,
            KeyCode.S => Keys.S,
            KeyCode.T => Keys.T,
            KeyCode.U => Keys.U,
            KeyCode.V => Keys.V,
            KeyCode.W => Keys.W,
            KeyCode.X => Keys.X,
            KeyCode.Y => Keys.Y,
            KeyCode.Z => Keys.Z,

            KeyCode.LeftWindows => Keys.LeftWindows,
            KeyCode.RightWindows => Keys.RightWindows,
            KeyCode.Apps => Keys.Apps,
            KeyCode.Sleep => Keys.Sleep,

            KeyCode.NumPad0 => Keys.NumPad0,
            KeyCode.NumPad1 => Keys.NumPad1,
            KeyCode.NumPad2 => Keys.NumPad2,
            KeyCode.NumPad3 => Keys.NumPad3,
            KeyCode.NumPad4 => Keys.NumPad4,
            KeyCode.NumPad5 => Keys.NumPad5,
            KeyCode.NumPad6 => Keys.NumPad6,
            KeyCode.NumPad7 => Keys.NumPad7,
            KeyCode.NumPad8 => Keys.NumPad8,
            KeyCode.NumPad9 => Keys.NumPad9,

            KeyCode.Multiply => Keys.Multiply,
            KeyCode.Add => Keys.Add,
            KeyCode.Separator => Keys.Separator,
            KeyCode.Subtract => Keys.Subtract,
            KeyCode.Decimal => Keys.Decimal,
            KeyCode.Divide => Keys.Divide,

            KeyCode.F1 => Keys.F1,
            KeyCode.F2 => Keys.F2,
            KeyCode.F3 => Keys.F3,
            KeyCode.F4 => Keys.F4,
            KeyCode.F5 => Keys.F5,
            KeyCode.F6 => Keys.F6,
            KeyCode.F7 => Keys.F7,
            KeyCode.F8 => Keys.F8,
            KeyCode.F9 => Keys.F9,
            KeyCode.F10 => Keys.F10,
            KeyCode.F11 => Keys.F11,
            KeyCode.F12 => Keys.F12,
            KeyCode.F13 => Keys.F13,
            KeyCode.F14 => Keys.F14,
            KeyCode.F15 => Keys.F15,
            KeyCode.F16 => Keys.F16,
            KeyCode.F17 => Keys.F17,
            KeyCode.F18 => Keys.F18,
            KeyCode.F19 => Keys.F19,
            KeyCode.F20 => Keys.F20,
            KeyCode.F21 => Keys.F21,
            KeyCode.F22 => Keys.F22,
            KeyCode.F23 => Keys.F23,
            KeyCode.F24 => Keys.F24,

            KeyCode.NumLock => Keys.NumLock,
            KeyCode.Scroll => Keys.Scroll,

            KeyCode.LeftShift => Keys.LeftShift,
            KeyCode.RightShift => Keys.RightShift,
            KeyCode.LeftControl => Keys.LeftControl,
            KeyCode.RightControl => Keys.RightControl,
            KeyCode.LeftAlt => Keys.LeftAlt,
            KeyCode.RightAlt => Keys.RightAlt,

            KeyCode.BrowserBack => Keys.BrowserBack,
            KeyCode.BrowserForward => Keys.BrowserForward,
            KeyCode.BrowserRefresh => Keys.BrowserRefresh,
            KeyCode.BrowserStop => Keys.BrowserStop,
            KeyCode.BrowserSearch => Keys.BrowserSearch,
            KeyCode.BrowserFavorites => Keys.BrowserFavorites,
            KeyCode.BrowserHome => Keys.BrowserHome,

            KeyCode.VolumeMute => Keys.VolumeMute,
            KeyCode.VolumeDown => Keys.VolumeDown,
            KeyCode.VolumeUp => Keys.VolumeUp,

            KeyCode.MediaNextTrack => Keys.MediaNextTrack,
            KeyCode.MediaPreviousTrack => Keys.MediaPreviousTrack,
            KeyCode.MediaStop => Keys.MediaStop,
            KeyCode.MediaPlayPause => Keys.MediaPlayPause,

            KeyCode.LaunchMail => Keys.LaunchMail,
            KeyCode.SelectMedia => Keys.SelectMedia,
            KeyCode.LaunchApplication1 => Keys.LaunchApplication1,
            KeyCode.LaunchApplication2 => Keys.LaunchApplication2,

            KeyCode.OemSemicolon => Keys.OemSemicolon,
            KeyCode.OemPlus => Keys.OemPlus,
            KeyCode.OemComma => Keys.OemComma,
            KeyCode.OemMinus => Keys.OemMinus,
            KeyCode.OemPeriod => Keys.OemPeriod,
            KeyCode.OemQuestion => Keys.OemQuestion,
            KeyCode.OemTilde => Keys.OemTilde,

            KeyCode.OemOpenBrackets => Keys.OemOpenBrackets,
            KeyCode.OemPipe => Keys.OemPipe,
            KeyCode.OemCloseBrackets => Keys.OemCloseBrackets,
            KeyCode.OemQuotes => Keys.OemQuotes,
            KeyCode.Oem8 => Keys.Oem8,

            KeyCode.OemBackslash => Keys.OemBackslash,

            KeyCode.ProcessKey => Keys.ProcessKey,
            KeyCode.Attn => Keys.Attn,
            KeyCode.Crsel => Keys.Crsel,
            KeyCode.Exsel => Keys.Exsel,
            KeyCode.EraseEof => Keys.EraseEof,
            KeyCode.Play => Keys.Play,
            KeyCode.Zoom => Keys.Zoom,
            KeyCode.Pa1 => Keys.Pa1,
            KeyCode.OemClear => Keys.OemClear,

            _ => Keys.None
        };
    }

    private static KeyCode FromFnaKey(Keys key)
    {
        return key switch
        {
            Keys.None => KeyCode.None,
            Keys.Back => KeyCode.Back,
            Keys.Tab => KeyCode.Tab,
            Keys.Enter => KeyCode.Enter,
            Keys.Pause => KeyCode.Pause,
            Keys.CapsLock => KeyCode.CapsLock,
            Keys.Escape => KeyCode.Escape,
            Keys.Space => KeyCode.Space,
            Keys.PageUp => KeyCode.PageUp,
            Keys.PageDown => KeyCode.PageDown,
            Keys.End => KeyCode.End,
            Keys.Home => KeyCode.Home,
            Keys.Left => KeyCode.Left,
            Keys.Up => KeyCode.Up,
            Keys.Right => KeyCode.Right,
            Keys.Down => KeyCode.Down,
            Keys.Select => KeyCode.Select,
            Keys.Print => KeyCode.Print,
            Keys.Execute => KeyCode.Execute,
            Keys.PrintScreen => KeyCode.PrintScreen,
            Keys.Insert => KeyCode.Insert,
            Keys.Delete => KeyCode.Delete,
            Keys.Help => KeyCode.Help,

            Keys.D0 => KeyCode.D0,
            Keys.D1 => KeyCode.D1,
            Keys.D2 => KeyCode.D2,
            Keys.D3 => KeyCode.D3,
            Keys.D4 => KeyCode.D4,
            Keys.D5 => KeyCode.D5,
            Keys.D6 => KeyCode.D6,
            Keys.D7 => KeyCode.D7,
            Keys.D8 => KeyCode.D8,
            Keys.D9 => KeyCode.D9,

            Keys.A => KeyCode.A,
            Keys.B => KeyCode.B,
            Keys.C => KeyCode.C,
            Keys.D => KeyCode.D,
            Keys.E => KeyCode.E,
            Keys.F => KeyCode.F,
            Keys.G => KeyCode.G,
            Keys.H => KeyCode.H,
            Keys.I => KeyCode.I,
            Keys.J => KeyCode.J,
            Keys.K => KeyCode.K,
            Keys.L => KeyCode.L,
            Keys.M => KeyCode.M,
            Keys.N => KeyCode.N,
            Keys.O => KeyCode.O,
            Keys.P => KeyCode.P,
            Keys.Q => KeyCode.Q,
            Keys.R => KeyCode.R,
            Keys.S => KeyCode.S,
            Keys.T => KeyCode.T,
            Keys.U => KeyCode.U,
            Keys.V => KeyCode.V,
            Keys.W => KeyCode.W,
            Keys.X => KeyCode.X,
            Keys.Y => KeyCode.Y,
            Keys.Z => KeyCode.Z,

            Keys.LeftWindows => KeyCode.LeftWindows,
            Keys.RightWindows => KeyCode.RightWindows,
            Keys.Apps => KeyCode.Apps,
            Keys.Sleep => KeyCode.Sleep,

            Keys.NumPad0 => KeyCode.NumPad0,
            Keys.NumPad1 => KeyCode.NumPad1,
            Keys.NumPad2 => KeyCode.NumPad2,
            Keys.NumPad3 => KeyCode.NumPad3,
            Keys.NumPad4 => KeyCode.NumPad4,
            Keys.NumPad5 => KeyCode.NumPad5,
            Keys.NumPad6 => KeyCode.NumPad6,
            Keys.NumPad7 => KeyCode.NumPad7,
            Keys.NumPad8 => KeyCode.NumPad8,
            Keys.NumPad9 => KeyCode.NumPad9,

            Keys.Multiply => KeyCode.Multiply,
            Keys.Add => KeyCode.Add,
            Keys.Separator => KeyCode.Separator,
            Keys.Subtract => KeyCode.Subtract,
            Keys.Decimal => KeyCode.Decimal,
            Keys.Divide => KeyCode.Divide,

            Keys.F1 => KeyCode.F1,
            Keys.F2 => KeyCode.F2,
            Keys.F3 => KeyCode.F3,
            Keys.F4 => KeyCode.F4,
            Keys.F5 => KeyCode.F5,
            Keys.F6 => KeyCode.F6,
            Keys.F7 => KeyCode.F7,
            Keys.F8 => KeyCode.F8,
            Keys.F9 => KeyCode.F9,
            Keys.F10 => KeyCode.F10,
            Keys.F11 => KeyCode.F11,
            Keys.F12 => KeyCode.F12,
            Keys.F13 => KeyCode.F13,
            Keys.F14 => KeyCode.F14,
            Keys.F15 => KeyCode.F15,
            Keys.F16 => KeyCode.F16,
            Keys.F17 => KeyCode.F17,
            Keys.F18 => KeyCode.F18,
            Keys.F19 => KeyCode.F19,
            Keys.F20 => KeyCode.F20,
            Keys.F21 => KeyCode.F21,
            Keys.F22 => KeyCode.F22,
            Keys.F23 => KeyCode.F23,
            Keys.F24 => KeyCode.F24,

            Keys.NumLock => KeyCode.NumLock,
            Keys.Scroll => KeyCode.Scroll,

            Keys.LeftShift => KeyCode.LeftShift,
            Keys.RightShift => KeyCode.RightShift,
            Keys.LeftControl => KeyCode.LeftControl,
            Keys.RightControl => KeyCode.RightControl,
            Keys.LeftAlt => KeyCode.LeftAlt,
            Keys.RightAlt => KeyCode.RightAlt,

            Keys.BrowserBack => KeyCode.BrowserBack,
            Keys.BrowserForward => KeyCode.BrowserForward,
            Keys.BrowserRefresh => KeyCode.BrowserRefresh,
            Keys.BrowserStop => KeyCode.BrowserStop,
            Keys.BrowserSearch => KeyCode.BrowserSearch,
            Keys.BrowserFavorites => KeyCode.BrowserFavorites,
            Keys.BrowserHome => KeyCode.BrowserHome,

            Keys.VolumeMute => KeyCode.VolumeMute,
            Keys.VolumeDown => KeyCode.VolumeDown,
            Keys.VolumeUp => KeyCode.VolumeUp,

            Keys.MediaNextTrack => KeyCode.MediaNextTrack,
            Keys.MediaPreviousTrack => KeyCode.MediaPreviousTrack,
            Keys.MediaStop => KeyCode.MediaStop,
            Keys.MediaPlayPause => KeyCode.MediaPlayPause,

            Keys.LaunchMail => KeyCode.LaunchMail,
            Keys.SelectMedia => KeyCode.SelectMedia,
            Keys.LaunchApplication1 => KeyCode.LaunchApplication1,
            Keys.LaunchApplication2 => KeyCode.LaunchApplication2,

            Keys.OemSemicolon => KeyCode.OemSemicolon,
            Keys.OemPlus => KeyCode.OemPlus,
            Keys.OemComma => KeyCode.OemComma,
            Keys.OemMinus => KeyCode.OemMinus,
            Keys.OemPeriod => KeyCode.OemPeriod,
            Keys.OemQuestion => KeyCode.OemQuestion,
            Keys.OemTilde => KeyCode.OemTilde,

            Keys.OemOpenBrackets => KeyCode.OemOpenBrackets,
            Keys.OemPipe => KeyCode.OemPipe,
            Keys.OemCloseBrackets => KeyCode.OemCloseBrackets,
            Keys.OemQuotes => KeyCode.OemQuotes,
            Keys.Oem8 => KeyCode.Oem8,

            Keys.OemBackslash => KeyCode.OemBackslash,

            Keys.ProcessKey => KeyCode.ProcessKey,
            Keys.Attn => KeyCode.Attn,
            Keys.Crsel => KeyCode.Crsel,
            Keys.Exsel => KeyCode.Exsel,
            Keys.EraseEof => KeyCode.EraseEof,
            Keys.Play => KeyCode.Play,
            Keys.Zoom => KeyCode.Zoom,
            Keys.Pa1 => KeyCode.Pa1,
            Keys.OemClear => KeyCode.OemClear,

            _ => KeyCode.None
        };
    }

    private static bool IsButtonDown(MouseState state, MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => state.LeftButton == ButtonState.Pressed,
            MouseButton.Right => state.RightButton == ButtonState.Pressed,
            MouseButton.Middle => state.MiddleButton == ButtonState.Pressed,
            MouseButton.XButton1 => state.XButton1 == ButtonState.Pressed,
            MouseButton.XButton2 => state.XButton2 == ButtonState.Pressed,

            // Gaming mouse buttons not supported by FNA:
            _ => false
        };
    }



    public void Update()
    {
        _prevKeyboard = _currKeyboard;
        _currKeyboard = Keyboard.GetState();

        _prevMouse = _currMouse;
        _currMouse = Mouse.GetState();
    }
}

