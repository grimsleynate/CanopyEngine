using ECS.Components;

namespace CanopyEngine.Physics;

public struct CircleCollider : IComponent, IComponentPOD
{
    public float Radius;
    public float OffsetX;
    public float OffsetY;
}