namespace ECS.Components;

public struct  StaticBody : IComponent, IComponentPOD
{
    public bool IsOneWay;
    public bool AffectedByGravity;
}