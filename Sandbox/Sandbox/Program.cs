using CanopyEngine;
using CanopyEngine.Platform.Abstractions;
using CanopyEngine.Platform.FNA;

public static class Program
{
    public static void Main()
    {
        using (var platform = new FnaPlatform(new WindowConfig(1920, 1080, "CanopyEngine Alpha Test")))
        {
            var engine = new Engine(platform);
            platform.SetEngine(engine);
            engine.Start();
            platform.Run();
        }
    }
}