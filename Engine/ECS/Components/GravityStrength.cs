namespace ECS.Components;

public struct GravityStrength : IComponent, IComponentPOD
{
    public float Value; // 1 = normal gravity, >1 = stronger gravity, <1 = weaker gravity, 0 = flying creatures etc.
}