using ECS.Systems;
using DefaultEcs.System;
using ECS.Components;
using Platform.Abstractions;
using Platform.FNA;
using Engine.Physics;
using Engine.World;
using System.Numerics;

public static class Program
{
    public static void Main()
    {
        using (var platform = new FnaPlatform(new WindowConfig(1920, 1080, "CanopyEngine Alpha Test")))
        {
            var engine = new CanopyEngine.Engine(platform);

            platform.SetEngine(engine);

            var map = new WorldTilemap { TileSize = 32 };
            var cam = new Engine.World.Camera { Position = new Vector2(200, 200), ViewWidth = 800, ViewHeight = 600, Zoom = 1f };
            TileRenderSystem? tileSystem = null;

            Sprite playerSprite = default;
            DefaultEcs.Entity? player = null;

            float x = 100;
            float y = 100;

            platform.Initialized += () =>
            {
                var assets = new FnaAssetManager(platform.GraphicsDevice);
                playerSprite = assets.LoadSprite("Content/player.png");

                tileSystem = new TileRenderSystem(map, cam, platform.Renderer);

                engine.SetDrawSystems(new SequentialSystem<float>(
                    new SpriteRenderSystem(engine.World, platform.Renderer)
                ));

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
                    new PlayerInputSystem(engine.World, platform.Input),
                    new HorizontalMovementSystem(engine.World)
                ));

                engine.SetFixedSystems(new SequentialSystem<float>(
                    new GravitySystem(engine.World, new WorldGravity()),
                    new PhysicsSystem(engine.World)
                ));
            };

            engine.OnUpdate += delta =>
            {
                if (platform.Input.IsKeyDown(KeyCode.Escape))
                    engine.Stop();
            };

            engine.OnDraw += () =>
            {
                tileSystem?.Draw();
                if (player.HasValue)
                {
                    var pos = player.Value.Get<Position>();
                    platform.Renderer.DrawSprite(playerSprite, new Vector2(pos.X, pos.Y), 0f, Color.White);
                }
            };

            platform.Run();
        }
    }
}
