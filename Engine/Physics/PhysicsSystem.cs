using CanopyEngine.Components;
using DefaultEcs;
using DefaultEcs.System;
using ECS.Components;
using System.Numerics;

namespace CanopyEngine.Physics;

public sealed class PhysicsSystem : ISystem<float>
{
    private readonly EntitySet _dynamicBodies;
    private readonly IStaticColliderProvider _staticAABBProvider;

    public bool IsEnabled { get; set; } = true;

    public PhysicsSystem(World world, IStaticColliderProvider colliderProvider)
    {
        _staticAABBProvider = colliderProvider;

        _dynamicBodies = world.GetEntities()
            .With<Position>()
            .With<Velocity>()
            .With<DynamicBody>()
            .AsSet();
    }

    public void Update(float fixedDelta)
    {
        foreach (var entity in _dynamicBodies.GetEntities())
        {
            ref Position pos = ref entity.Get<Position>();
            ref Velocity vel = ref entity.Get<Velocity>();

            float dx = vel.X * fixedDelta;
            float dy = vel.Y * fixedDelta;

            float remaining = 1f;
            int iterations = 0;

            while (remaining > 0f && iterations++ < 4 && (dx != 0f || dy != 0f))
            {
                float earliestT = 1f;
                Vector2 hitNormal = Vector2.Zero;
                bool hitSomething = false;

                // -----------------------------
                // Compute circle center
                // -----------------------------

                if (!entity.Has<CircleCollider>())
                {
                    // No collider → just move
                    pos.X += dx;
                    pos.Y += dy;
                    break;
                }

                ref CircleCollider circle = ref entity.Get<CircleCollider>();

                Vector2 center = new Vector2(
                    pos.X + circle.OffsetX,
                    pos.Y + circle.OffsetY
                );
                float radius = circle.Radius;

                // Compute swept bounds
                float minX = MathF.Min(center.X, center.X + dx) - radius;
                float minY = MathF.Min(center.Y, center.Y + dy) - radius;
                float maxX = MathF.Max(center.X, center.X + dx) + radius;
                float maxY = MathF.Max(center.Y, center.Y + dy) + radius;

                // -----------------------------
                // Query tile colliders
                // -----------------------------
                var colliders = _staticAABBProvider.QueryAABBs(minX, minY, maxX, maxY);

                //TEMP DEBUG
                int count = 0;
                foreach (var _ in colliders) count++;
                Console.WriteLine($"Physics query: bounds=({minX},{minY})-({maxX},{maxY}), colliders={count}");

                foreach (var colBox in colliders)
                {
                    SweepResult sweep = PhysicsMath.SweepCircleAgainstAabb(
                        center,
                        radius,
                        colBox,
                        dx,
                        dy
                    );

                    if (sweep.Hit)
                    {
                        Console.WriteLine($"Hit AABB: {colBox.MinX},{colBox.MinY} -> {colBox.MaxX},{colBox.MaxY}, toi={sweep.TimeOfImpact}, normal={sweep.Normal}");
                    }

                    if (!sweep.Hit)
                        continue;

                    if (sweep.TimeOfImpact < earliestT)
                    {
                        earliestT = sweep.TimeOfImpact;
                        hitNormal = sweep.Normal;
                        hitSomething = true;
                    }
                }

                // -----------------------------
                // No collision → move fully
                // -----------------------------
                if (!hitSomething)
                {
                    pos.X += dx;
                    pos.Y += dy;
                    break;
                }

                // -----------------------------
                // Move to collision point
                // -----------------------------
                pos.X += dx * earliestT;
                pos.Y += dy * earliestT;

                // Push out slightly to avoid sticking
                const float epsilon = 0.001f;
                pos.X += hitNormal.X * epsilon;
                pos.Y += hitNormal.Y * epsilon;

                remaining -= earliestT * remaining;

                // -----------------------------
                // Slide along surface
                // -----------------------------
                Vector2 velVec = new Vector2(vel.X, vel.Y);
                float vn = Vector2.Dot(velVec, hitNormal);

                if (vn < 0f)
                {
                    velVec -= vn * hitNormal;
                    vel.X = velVec.X;
                    vel.Y = velVec.Y;
                }

                dx = vel.X * fixedDelta * remaining;
                dy = vel.Y * fixedDelta * remaining;
            }
        }
    }

    public void Dispose()
    {
        _dynamicBodies.Dispose();
    }
}
