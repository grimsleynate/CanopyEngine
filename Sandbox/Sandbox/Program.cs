using CanopyEngine;
using CanopyEngine.Platform.Abstractions;
using CanopyEngine.Platform.FNA;

public static class Program
{
    public static void Main()
    {
        var platform = new FnaPlatform(new WindowConfig(1280, 720, "CanopyEngine Alpha Test"));
        var engine = new Engine(platform);

        platform.SetEngine(engine);
        platform.Run();
    }
}