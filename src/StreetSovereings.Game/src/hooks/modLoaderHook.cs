namespace StreetSovereings.Game.hooks;

internal class ModLoaderHook
{
    internal static bool CheckModLoader()
    {
        return File.Exists("./SSModLoader.exe");
    }
}