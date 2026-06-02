using CanopyEngine;
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

            Sprite sprite = default;
            float x = 100;
            float y = 100;

            platform.Initialized += () =>
            {
                var assets = new FnaAssetManager(platform.GraphicsDevice);
                sprite = assets.LoadSprite("Content/player.png");
            };

            engine.OnUpdate += delta =>
            {
                x += 50f * delta;

                if (platform.Input.IsKeyDown(KeyCode.Escape))
                    engine.Stop();
            };

            engine.OnDraw += () =>
            {
                if (sprite.IsValid)
                {
                    platform.Renderer.DrawSprite(sprite, new System.Numerics.Vector2(x, y), 0f, Color.White);
                }
            };

            engine.Start();
            platform.Run();
        }
    }
}
