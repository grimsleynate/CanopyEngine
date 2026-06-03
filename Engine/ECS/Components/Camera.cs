using System.Numerics;

namespace ECS.Components;

public struct Camera : IComponent, IComponentPOD
{
    public Vector2 position;
    public int ViewWidth;
    public int ViewHeight;
    public float Zoom;
}