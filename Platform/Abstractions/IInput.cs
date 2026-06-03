using System.Numerics;

namespace Platform.Abstractions;

public interface IInput
{
    void Update();

    // Polling queries
    bool IsKeyDown(KeyCode key);
    bool IsKeyPressed(KeyCode key);   // true only on the frame the key went down
    bool IsKeyReleased(KeyCode key);  // true only on the frame the key went up

    bool IsMouseDown(MouseButton button);
    bool IsMousePressed(MouseButton button);
    bool IsMouseReleased(MouseButton button);

    // Mouse position in screen coordinates
    Vector2 MousePosition { get; }

    // Convenience read-only views for systems that want to iterate
    IReadOnlyList<KeyCode> KeysDown { get; }
    IReadOnlyList<KeyCode> KeysPressedThisFrame { get; }
    IReadOnlyList<KeyCode> KeysReleasedThisFrame { get; }

    IReadOnlyList<MouseButton> MouseButtonsDown { get; }
    IReadOnlyList<MouseButton> MouseButtonsPressedThisFrame { get; }
    IReadOnlyList<MouseButton> MouseButtonsReleasedThisFrame { get; }

    // Optional convenience axis helpers (keyboard only for now)
    float GetAxis(string axisName); // "Horizontal" / "Vertical"
}