using Engine.Physics;
using System.Numerics;
using Platform.Abstractions;
using DefaultEcs.System;

namespace Engine.World;

// A simple tile renderer that culls by visible chunk range and draws repeating tiles
public class TileRenderSystem
{
    private readonly WorldTilemap _map;
    private readonly Camera _camera;
    private readonly IRenderer _renderer;
    private readonly uint _floorColor = 0xFF8B5A2B; // example ARGB color
    private readonly uint _wallColor = 0xFF2F4F4F;

    public TileRenderSystem(WorldTilemap map, Camera camera, IRenderer renderer)
    {
        _map = map ?? throw new ArgumentNullException(nameof(map));
        _camera = camera;
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    public void Draw()
    {
        var (minCx, minCy, maxCx, maxCy) = _map.GetVisibleChunkRange(_camera);

        for (int cx = minCx; cx <= maxCx; cx++)
            for (int cy = minCy; cy <= maxCy; cy++)
            {
                var chunk = _map.GetOrCreateChunk(cx, cy, PrototypeChunkGenerator);
                DrawChunk(chunk);
            }
    }

    private void DrawChunk(Chunk chunk)
    {
        float tileSize = _map.TileSize;
        float baseX = chunk.CX * Chunk.Size * tileSize;
        float baseY = chunk.CY * Chunk.Size * tileSize;

        for (int ly = 0; ly < Chunk.Size; ly++)
            for (int lx = 0; lx < Chunk.Size; lx++)
            {
                var tile = chunk.GetTile(lx, ly);
                var worldX = baseX + lx * tileSize;
                var worldY = baseY + ly * tileSize;
                var pos = new Vector2(worldX, worldY);

                if (tile.Type == TileType.Floor)
                    _renderer.DrawQuad(pos, tileSize, tileSize, _floorColor);
                else if (tile.Type == TileType.Wall)
                    _renderer.DrawQuad(pos, tileSize, tileSize, _wallColor);
            }
    }

    private bool PrototypeChunkGenerator(Chunk chunk)
    {
        for (int y = 0; y < Chunk.Size; y++)
            for (int x = 0; x < Chunk.Size; x++)
            {
                var t = new Tile { Type = TileType.Floor, SpriteId = 1, Flags = 0, Height = 0f };
                chunk.SetTile(x, y, t);
            }
        TileRenderSystem.BuildCollidersForChunk(chunk, _map.TileSize, tile => (tile.Flags & 1) != 0);
        return true;
    }

    // Simple horizontal run merging into AABBs
    private static void BuildCollidersForChunk(Chunk chunk, int tileSize, Func<Tile, bool> isCollidable)
    {
        chunk.Colliders.Clear();
        for (int y = 0; y < Chunk.Size; y++)
        {
            int runStart = -1;
            for (int x = 0; x <= Chunk.Size; x++)
            {
                bool coll = (x < Chunk.Size) && isCollidable(chunk.GetTile(x, y));
                if (coll && runStart == -1) runStart = x;
                if ((!coll || x == Chunk.Size) && runStart != -1)
                {
                    int runEnd = x - 1;
                    float worldX = (chunk.CX * Chunk.Size + runStart) * tileSize;
                    float worldY = (chunk.CY * Chunk.Size + y) * tileSize;
                    float width = (runEnd - runStart + 1) * tileSize;
                    chunk.Colliders.Add(new AABB
                    {
                        MinX = worldX,
                        MinY = worldY,
                        MaxX = worldX + width,
                        MaxY = worldY + tileSize
                    });
                    runStart = -1;
                }
            }
        }
    }
}