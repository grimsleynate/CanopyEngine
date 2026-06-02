using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;

namespace ECS.Systems;

public sealed class HorizontalMovementSystem : AEntitySetSystem<float>
{
    public HorizontalMovementSystem(World world)
        : base(world.GetEntities()
              .With<Position>()
              .With<Velocity>()
              .AsSet())
    {
    }
    protected override void Update(float delta, in Entity entity)
    {
        ref Position pos = ref entity.Get<Position>();
        ref Velocity vel = ref entity.Get<Velocity>();
        pos.X += vel.X * delta;
        pos.Y += vel.Y * delta;
    }
}