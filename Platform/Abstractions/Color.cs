namespace Platform.Abstractions;

public readonly struct Color
{
    public float R { get; }
    public float G { get; }
    public float B { get; }
    public float A { get; }
    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    public static readonly Color White = new Color(1f, 1f, 1f, 1f);
    public static readonly Color Black = new Color(0f, 0f, 0f, 1f);
    public static readonly Color Red = new Color(1f, 0f, 0f, 1f);
    public static readonly Color Green = new Color(0f, 1f, 0f, 1f);
    public static readonly Color Blue = new Color(0f, 0f, 1f, 1f);
}