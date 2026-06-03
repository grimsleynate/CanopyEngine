using System.Numerics;
using ECS.Components;

namespace Engine.World;

public struct Camera : IComponent, IComponentPOD
{
    public Vector2 Position;
    public int ViewWidth;
    public int ViewHeight;
    public float Zoom;
}