namespace CanopyEngine.Platform.Abstractions;
public interface IInput
{
    void Update();

    bool IsKeyDown(KeyCode key);
    bool IsMouseDown(MouseButton button);
    (int X, int Y) MousePosition { get; }

    IReadOnlyList<KeyCode> KeysDown { get; }
    IReadOnlyList<KeyCode> KeysPressedThisFrame { get; }
    IReadOnlyList<KeyCode> KeysReleasedThisFrame { get; }

    IReadOnlyList<MouseButton> MouseButtonsDown { get; }
    IReadOnlyList<MouseButton> MouseButtonsPressedThisFrame { get; }
    IReadOnlyList<MouseButton> MouseButtonsReleasedThisFrame { get; }
}