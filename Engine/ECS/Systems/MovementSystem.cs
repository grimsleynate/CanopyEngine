using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;

namespace ECS.Systems;

public sealed class MovementSystem : AEntitySetSystem<float>
{
    public MovementSystem(World world)
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
        pos.X += vel.VX * delta;
        pos.Y += vel.VY * delta;
    }
}