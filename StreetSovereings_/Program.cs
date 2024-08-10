using OpenTK.Mathematics;
using StreetSovereigns.src.hooks;
using StreetSovereings_.src;
using StreetSovereings_.src.ModLoader;

namespace StreetSovereigns
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            Console.WriteLine("Loading...");
            using (var game = new Renderer.Game())
            {
                Console.WriteLine("Mod Loader Status: " + ModLoaderHook.CheckModLoader());
                if (ModLoaderHook.CheckModLoader())
                {
                    ModLoader.Load();
                }
                else
                {
                    ModLoader.NotLoad();
                }
                Console.WriteLine("Game runned!");
                game.AddCube(0.0f, 1.0f, 0.0f, new Vector4(1.0f, 1.0f, 0.0f, 1.0f), 1.0f);
                game.Run();
            }
        }
    }
}