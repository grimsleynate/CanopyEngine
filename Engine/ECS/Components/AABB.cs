namespace ECS.Components;

public struct AABB : IComponent, IComponentPOD
{
    public float MinX, MinY;
    public float MaxX, MaxY;
}