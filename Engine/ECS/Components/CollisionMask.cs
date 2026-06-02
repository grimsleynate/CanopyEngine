namespace ECS.Components;

public struct CollisionMask : IComponent, IComponentPOD
{
    public int Layer;
    public int Mask;
}
