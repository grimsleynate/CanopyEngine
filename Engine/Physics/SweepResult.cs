using System.Numerics;
using Engine.Physics;
using ECS.Components;

namespace Engine.Components;

public readonly struct SweepResult : IComponent
{
    public readonly bool Hit;
    public readonly float TimeOfImpact;
    public readonly Vector2 Normal;

    public SweepResult(bool hit, float toi, Vector2 normal)
    {
        Hit = hit;
        TimeOfImpact = toi;
        Normal = normal;
    }

    public static SweepResult NoHit => new(false, 1f, Vector2.Zero);

    public static SweepResult SweepAabb(
    in AABB moving,
    in AABB target,
    float dx,
    float dy)
    {
        if (PhysicsMath.AabbOverlap(moving, target))
            return SweepResult.NoHit;

        float xEntry, xExit;
        float yEntry, yExit;

        if (dx > 0f)
        {
            xEntry = target.MinX - moving.MaxX;
            xExit = target.MaxX - moving.MinX;
        }
        else if (dx < 0f)
        {
            xEntry = target.MaxX - moving.MinX;
            xExit = target.MinX - moving.MaxX;
        }
        else
        {
            xEntry = float.NegativeInfinity;
            xExit = float.PositiveInfinity;
        }

        if (dy > 0f)
        {
            yEntry = target.MinY - moving.MaxY;
            yExit = target.MaxY - moving.MinY;
        }
        else if (dy < 0f)
        {
            yEntry = target.MaxY - moving.MinY;
            yExit = target.MinY - moving.MaxY;
        }
        else
        {
            yEntry = float.NegativeInfinity;
            yExit = float.PositiveInfinity;
        }

        float txEntry = (dx == 0f) ? float.NegativeInfinity : xEntry / dx;
        float txExit = (dx == 0f) ? float.PositiveInfinity : xExit / dx;

        float tyEntry = (dy == 0f) ? float.NegativeInfinity : yEntry / dy;
        float tyExit = (dy == 0f) ? float.PositiveInfinity : yExit / dy;

        float tEntry = Math.Max(txEntry, tyEntry);
        float tExit = Math.Min(txExit, tyExit);

        if (tEntry > tExit || tEntry < 0f || tEntry > 1f)
            return SweepResult.NoHit;

        Vector2 normal;

        if (txEntry > tyEntry)
            normal = (dx > 0f) ? new Vector2(-1, 0) : new Vector2(1, 0);
        else
            normal = (dy > 0f) ? new Vector2(0, -1) : new Vector2(0, 1);

        return new SweepResult(true, tEntry, normal);
    }
}
