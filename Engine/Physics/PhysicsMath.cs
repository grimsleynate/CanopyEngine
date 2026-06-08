using ECS.Components;
using CanopyEngine.Components;
using System.Numerics;

namespace CanopyEngine.Physics;

public class PhysicsMath
{
    public static SweepResult SweepAabb(
    in AABB moving,
    in AABB target,
    float dx,
    float dy)
    {
        if (AabbOverlap(moving, target))
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

    public static bool AabbOverlap(in AABB a, in AABB b)
    {
        return a.MinX < b.MaxX &&
               a.MaxX > b.MinX &&
               a.MinY < b.MaxY &&
               a.MaxY > b.MinY;
    }

    public static Vector2 ClosestPointOnAabb(in AABB box, in Vector2 point)
    {
        float x = MathF.Max(box.MinX, MathF.Min(point.X, box.MaxX));
        float y = MathF.Max(box.MinY, MathF.Min(point.Y, box.MaxY));
        return new Vector2(x, y);
    }

    public static bool CircleOverlapsAabb(in Vector2 center, float radius, in AABB box)
    {
        Vector2 closest = ClosestPointOnAabb(box, center);
        Vector2 diff = center - closest;
        return diff.LengthSquared() < radius * radius;
    }

    public static SweepResult SweepCircleAgainstAabb(in Vector2 center, float radius, in AABB target, float dx, float dy)
    {
        // Broad reject: if no movement, just overlap test
        if (dx == 0f && dy == 0f)
        {
            bool hit = CircleOverlapsAabb(center, radius, target);
            return hit ? new SweepResult(true, 0f, Vector2.Zero) : SweepResult.NoHit;
        }

        // Expand target by circle radius and sweep point
        AABB expanded = new AABB
        {
            MinX = target.MinX - radius,
            MinY = target.MinY - radius,
            MaxX = target.MaxX + radius,
            MaxY = target.MaxY + radius
        };

        AABB movingPoint = new AABB
        {
            MinX = center.X,
            MinY = center.Y,
            MaxX = center.X,
            MaxY = center.Y
        };

        SweepResult sweep = SweepAabb(movingPoint, expanded, dx, dy);
        if (!sweep.Hit)
            return SweepResult.NoHit;

        // Compute normal from closest point at impact
        float hitX = center.X + dx * sweep.TimeOfImpact;
        float hitY = center.Y + dy * sweep.TimeOfImpact;
        Vector2 hitCenter = new Vector2(hitX, hitY);
        Vector2 closest = ClosestPointOnAabb(target, hitCenter);
        Vector2 normal = hitCenter - closest;

        if (normal.LengthSquared() > 0.0001f)
            normal = Vector2.Normalize(normal);
        else
            normal = sweep.Normal; // fallback to AABB normal

        return new SweepResult(true, sweep.TimeOfImpact, normal);
    }
}
