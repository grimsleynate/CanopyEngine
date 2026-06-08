using CanopyEngine.Physics;

namespace CanopyEngine.World;

public struct Tile
{
    public TileType Type;
    public ushort SpriteId;
    public byte Flags;
    public float Height;
}

public class Chunk
{
    public const int Size = 32;
    public readonly Tile[] Tiles;
    public readonly List<AABB> Colliders;
    public bool IsLoaded;
    public int CX, CY;

    public Chunk(int cx, int cy)
    {
        CX = cx; 
        CY = cy;
        Tiles = new Tile[Size * Size];
        Colliders = new List<AABB>();
        IsLoaded = false;
    }

    public Tile GetTile(int lx, int ly) => Tiles[ly  * Size + lx];
    public void SetTile(int lx, int ly, Tile tile) => Tiles[ly * Size + lx] = tile;

    public void BuildColliders(WorldTilemap map)
    {
        int tileSize = map.TileSize;
        float chunkWorldX = CX * Size * tileSize;
        float chunkWorldY = CY * Size * tileSize;

        for (int ly = 0; ly < Size; ly++)
        {
            for (int lx = 0; lx < Size; lx++)
            {
                Tile tile = GetTile(lx, ly);
                if (tile.Type != TileType.Wall)
                    continue;

                float minX = chunkWorldX + lx * tileSize;
                float minY = chunkWorldY + ly * tileSize;
                float maxX = minX + tileSize;
                float maxY = minY + tileSize;

                Colliders.Add(new AABB
                {
                    MinX = minX,
                    MinY = minY,
                    MaxX = maxX,
                    MaxY = maxY
                });
            }
        }

    }
}
