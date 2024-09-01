using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using StreetSovereings.Game.hooks;

namespace StreetSovereings.Game;

public class Program
{
    private static GameWindow? _window;
        
    [STAThread]
    public static void Main()
    {
        ShowRenderer();
    }

    public static void ShowRenderer()
    {
        Console.WriteLine("Loading...");
        using (var game = new Renderer.Game())
        {
            _window = game;
            ConfigHook config = new ConfigHook();
            Console.WriteLine($"ConfigEnabled: {config.GetConfig()}");
            Console.WriteLine("Mod Loader Status: " + ModLoaderHook.CheckModLoader());
            if (ModLoaderHook.CheckModLoader())
            {
                ModLoader.ModLoader.Load();
            }
            else
            {
                ModLoader.ModLoader.NotLoad();
            }
            Console.WriteLine("Game runned!");
            game.AddCube(0.0f, 1.0f, 0.0f, new Vector4(1.0f, 1.0f, 0.0f, 1.0f), 1.0f);
            game.Run();
        }
    }

    public static void Destroy()
    {
        _window?.Close();
    }
}