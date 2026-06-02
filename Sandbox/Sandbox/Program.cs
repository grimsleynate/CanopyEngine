using CanopyEngine;
using ECS.Systems;
using DefaultEcs.System;
using ECS.Components;
using Platform.Abstractions;
using Platform.FNA;

public static class Program
{
    public static void Main()
    {
        using (var platform = new FnaPlatform(new WindowConfig(1920, 1080, "CanopyEngine Alpha Test")))
        {
            var engine = new Engine(platform);
            platform.SetEngine(engine);

            Sprite playerSprite = default;
            float x = 100;
            float y = 100;

            platform.Initialized += () =>
            {
                var assets = new FnaAssetManager(platform.GraphicsDevice);
                playerSprite = assets.LoadSprite("Content/player.png");

                engine.SetDrawSystems(new SequentialSystem<float>(
                    new SpriteRenderSystem(engine.World, platform.Renderer)
                ));

                engine.Start();
            };

            engine.OnStart += () =>
            {
                var player = engine.World.CreateEntity();

                player.Set(new Position { X = x, Y = y });
                player.Set(new Velocity { X = 0, Y = 0 });
                player.Set(new SpriteComponent { Sprite = playerSprite });
                player.Set(new PlayerTag());
                player.Set(new GravityStrength { Value = 900f });

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
                
            };

            platform.Run();
        }
    }
}
