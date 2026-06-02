using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;

namespace ECS.Systems;

public sealed class GravitySystem : AEntitySetSystem<float>
{
    public GravitySystem(World world)
        : base(world.GetEntities()
              .With<Velocity>()
              .With<Gravity>()
              .AsSet())
    { }

    protected override void Update(float delta, in Entity entity)
    {
        ref Velocity vel = ref entity.Get<Velocity>();
        ref Gravity grav = ref entity.Get<Gravity>();

        vel.VY += grav.Strength * delta;
    }
}