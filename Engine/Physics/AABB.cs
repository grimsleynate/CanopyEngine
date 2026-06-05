using ECS.Components;

namespace CanopyEngine.Physics;

public struct AABB : IComponent, IComponentPOD
{
    public float MinX, MinY;
    public float MaxX, MaxY;

    public float Width => MaxX - MinX;
    public float Height => MaxY - MinY;
}
