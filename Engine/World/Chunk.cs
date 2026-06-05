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
}
