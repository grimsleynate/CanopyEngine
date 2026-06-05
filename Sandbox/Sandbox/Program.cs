using DefaultEcs.System;
using ECS.Components;
using ECS.Systems;
using CanopyEngine.Debugging;
using CanopyEngine.World;
using Microsoft.Xna.Framework.Graphics;
using Platform.Abstractions;
using Platform.FNA;
using System.Numerics;
using CanopyEngine;
using CanopyEngine.Physics;
using CanopyEngine.Profiling;

public static class Program
{
    public static void Main()
    {
        using (var platform = new FnaPlatform(new WindowConfig(1920, 1080, "CanopyEngine Alpha Test")))
        {
            // helper local
            static ISystem<float> WrapTimed(ISystem<float> sys, string name) => new TimedSystem(sys, name);

            var engine = new Engine(platform);

            platform.SetEngine(engine);

            var map = new WorldTilemap { TileSize = 32 };
            var cam = new CanopyEngine.World.Camera { Position = new Vector2(200, 200), ViewWidth = 800, ViewHeight = 600, Zoom = 1f };
            TileRenderSystem? tileSystem = null;

            Sprite playerSprite = default;
            DefaultEcs.Entity? player = null;
            DefaultEcs.EntitySet? _allEntities = default;

            DebugOverlayRenderer? debugOverlay = null;

            float x = 100;
            float y = 100;

            platform.Initialized += () =>
            {
                var assets = new FnaAssetManager(platform.GraphicsDevice);
                playerSprite = assets.LoadSprite("Content/player.png");

                SpriteFont debugFont = platform.Content.Load<SpriteFont>("Fonts/DebugFont");

                tileSystem = new TileRenderSystem(map, cam, platform.Renderer);

                engine.SetDrawSystems(new SequentialSystem<float>(
                    new SpriteRenderSystem(engine.World, platform.Renderer)
                ));

                debugOverlay = new DebugOverlayRenderer(
                    platform.Input, 
                    platform.Time, 
                    debugFont, 
                    (FnaRenderer)platform.Renderer
                );
                _allEntities = engine.World.GetEntities().AsSet();

                engine.Start();
            };

            engine.OnStart += () =>
            {
                player = engine.World.CreateEntity();

                player.Value.Set(new Position { X = x, Y = y });
                player.Value.Set(new Velocity { X = 0, Y = 0 });
                player.Value.Set(new SpriteComponent { Sprite = playerSprite });
                player.Value.Set(new PlayerTag());
                player.Value.Set(new GravityStrength { Value = 900f });

                engine.SetUpdateSystems(new SequentialSystem<float>(
                    WrapTimed(new PlayerInputSystem(engine.World, platform.Input, moveSpeed: 200f), "Player Input"),
                    WrapTimed(new MovementSystem(engine.World), "Movement")
                ));

                engine.SetFixedSystems(new SequentialSystem<float>(
                    WrapTimed(new GravitySystem(engine.World, new WorldGravity()), "Gravity"),
                    WrapTimed(new PhysicsSystem(engine.World), "Physics")
                ));
            };

            engine.OnUpdate += delta =>
            {
                if (platform.Input.IsKeyDown(KeyCode.Escape))
                    engine.Stop();

                if (debugOverlay != null && platform.Input.IsKeyPressed(KeyCode.F3))
                {
                    debugOverlay.UpdateToggle();
                    Console.WriteLine("F3 was pressed.");
                }
            };

            engine.OnDraw += () =>
            {
                tileSystem?.Draw();
                if (player.HasValue)
                {
                    var pos = player.Value.Get<Position>();
                    platform.Renderer.DrawSprite(playerSprite, new Vector2(pos.X, pos.Y), 0f, Color.White);
                }

                if (debugOverlay != null && _allEntities != null)
                    debugOverlay.Draw(platform.GraphicsDevice, _allEntities.Count);
            };

            platform.Run();
        }
    }
}
