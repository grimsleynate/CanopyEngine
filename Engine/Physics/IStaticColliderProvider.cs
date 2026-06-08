namespace CanopyEngine.Physics;

public interface IStaticColliderProvider
{
    // Returns all AABBs that might collide with the given swept bounds
    IEnumerable<AABB> QueryAABBs(float minX, float minY, float maxX, float maxY);
}
