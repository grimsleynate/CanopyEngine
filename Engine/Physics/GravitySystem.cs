using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;

namespace CanopyEngine.Physics;

public sealed class GravitySystem : AEntitySetSystem<float>
{
    private readonly WorldGravity _worldGravity;

    public GravitySystem(World world, WorldGravity worldGravity)
        : base(world.GetEntities()
              .With<Height>()
              .With<GravityStrength>()
              .With<Mass>()
              .AsSet())
    {
        _worldGravity = worldGravity;
    }

    protected override void Update(float delta, in Entity entity)
    {
        ref Height height = ref entity.Get<Height>();
        ref GravityStrength grav = ref entity.Get<GravityStrength>();
        ref Mass mass = ref entity.Get<Mass>();

        // F = m * a → a = F/m
        float accel = grav.Value * _worldGravity.Value / mass.Value;

        height.VelocityZ += accel * delta;
    }
}