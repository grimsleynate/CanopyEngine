using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace ECS.Systems;

public sealed class PhysicsSystem : ISystem<float>
{
    private readonly EntitySet _dynamicBodies;
    private readonly EntitySet _staticBodies;

    public bool IsEnabled { get; set; } = true;

    public PhysicsSystem(World world)
    {
        _dynamicBodies = world.GetEntities()
            .With<Position>()
            .With<Velocity>()
            .With<AABB>()
            .With<DynamicBody>()
            .AsSet();

        _staticBodies = world.GetEntities()
            .With<AABB>()
            .With<StaticBody>()
            .AsSet();
    }

    public void Update(float fixedDelta)
    {
        foreach (var entity in _dynamicBodies.GetEntities())
        {
            ref Position pos = ref entity.Get<Position>();
            ref Velocity vel = ref entity.Get<Velocity>();
            ref AABB box = ref entity.Get<AABB>();
            ref DynamicBody body = ref entity.Get<DynamicBody>();

            //Predict motion
            float dx = vel.VX * fixedDelta;
            float dy = vel.VY * fixedDelta;

            foreach (var collider in _staticBodies.GetEntities())
            {
                ref AABB colBox = ref collider.Get<AABB>();
                ref StaticBody colBody = ref collider.Get<StaticBody>();

                //Swept AABB narrowphase goes here
            }

            //Resolve movement
            pos.X += dx;
            pos.Y += dy;
        }
    }

    public void Dispose()
    {
        _dynamicBodies.Dispose();
        _staticBodies.Dispose();
    }
}