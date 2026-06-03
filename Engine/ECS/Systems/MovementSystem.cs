using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;

namespace ECS.Systems;

public sealed class MovementSystem : AEntitySetSystem<float>
{
    private readonly float _maxStep;

    public MovementSystem(World world, float maxStep = float.PositiveInfinity)
        : base(world.GetEntities().With<Position>().With<Velocity>().AsSet())
    {
        _maxStep = maxStep;
    }

    protected override void Update(float delta, in Entity entity)
    {
        var dt = MathF.Min(delta, _maxStep);
        ref Position pos = ref entity.Get<Position>();
        ref Velocity vel = ref entity.Get<Velocity>();

        pos.X += vel.X * dt;
        pos.Y += vel.Y * dt;
    }
}
