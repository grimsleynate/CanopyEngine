using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;

namespace Engine.Physics;

public sealed class VerticalMovementSystem : AEntitySetSystem<float>
{
    public VerticalMovementSystem(World world)
        : base(world.GetEntities()
              .With<Height>()
              .AsSet())
    {
    }
    protected override void Update(float delta, in Entity entity)
    {
        ref Height height = ref entity.Get<Height>();

        height.Z += height.VelocityZ * delta;

        // Landing on ground
        if (height.Z <= 0f)
        {
            height.Z = 0f;
            height.VelocityZ = 0f;

            entity.Set(new Grounded { Value = true });
        }
        else
        {
            entity.Set(new Grounded { Value = false });
        }
    }
}