using CanopyEngine.World;
using CanopyEngine.Physics;

namespace CanopyEngine.World;

public class WorldTilemap : IStaticColliderProvider
{
    public readonly Dictionary<(int cx, int cy), Chunk> Chunks = new();
    public int TileSize = 32;
    public int LoadedRadiusInChunks = 2;

    // Convert world position (x,y) to chunk coords
    public (int cx, int cy) WorldToChunkCoords(float worldX, float worldY)
    {
        int cx = (int)Math.Floor(worldX / (TileSize * Chunk.Size));
        int cy = (int)Math.Floor(worldY / (TileSize * Chunk.Size));
        return (cx, cy);
    }

    // Convert world rect to chunk range inclusive
    public (int minCx, int minCy, int maxCx, int maxCy) WorldRectToChunkRange(float minX, float minY, float maxX, float maxY)
    {
        var (minCx, minCy) = WorldToChunkCoords(minX, minY);
        var (maxCx, maxCy) = WorldToChunkCoords(maxX, maxY);

        if (maxCx < minCx) (minCx, maxCx) = (maxCx, minCx);
        if (maxCy < minCy) (minCy, maxCy) = (maxCy, minCy);

        return (minCx, minCy, maxCx, maxCy);
    }

    // Compute visible chunk range from camera bounds
    public (int minCx, int minCy, int maxCx, int maxCy) GetVisibleChunkRange(Camera cam)
    {
        float halfW = cam.ViewWidth * 0.5f / cam.Zoom;
        float halfH = cam.ViewHeight * 0.5f / cam.Zoom;

        float left = cam.Position.X - halfW;
        float right = cam.Position.X + halfW;
        float top = cam.Position.Y - halfH;
        float bottom = cam.Position.Y + halfH;

        float expand = TileSize * Chunk.Size;

        return WorldRectToChunkRange(left - expand, top - expand, right + expand, bottom + expand);
    }

    // Get or create a chunk (synchronous prototype generator)
    public Chunk GetOrCreateChunk(int cx, int cy, Func<Chunk, bool> generator = null)
    {
        if (!Chunks.TryGetValue((cx, cy), out var chunk))
        {
            chunk = new Chunk(cx, cy);
            Chunks[(cx, cy)] = chunk;
            if (generator != null)
                generator(chunk);
            chunk.IsLoaded = true;
        }
        return chunk;
    }

    public IEnumerable<AABB> QueryAABBs(float minX, float minY, float maxX, float maxY)
    {
        var (minCx, minCy, maxCx, maxCy) = WorldRectToChunkRange(minX, minY, maxX, maxY);

        for (int cy = minCy; cy <= maxCy; cy++)
            for (int cx = minCx; cx <= maxCx; cx++)
            {
                if (!Chunks.TryGetValue((cx, cy), out var chunk) || !chunk.IsLoaded)
                    continue;

                foreach (var col in chunk.Colliders)
                    yield return col;
            }
    }
}