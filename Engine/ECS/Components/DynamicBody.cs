namespace ECS.Components;

public struct  DynamicBody : IComponent, IComponentPOD
{
    public bool IsOneWay;
    public bool AffectedByGravity;
}