namespace ECS.Components;

public struct Mass : IComponent, IComponentPOD
{
    public float Value; // 1 = normal, >1 = heavier, <1 = lighter
}