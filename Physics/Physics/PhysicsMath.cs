using ECS.Components;
using System.Numerics;

namespace Physics;

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

}
