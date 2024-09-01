using System.Text.Json;

namespace StreetSovereings.Game.hooks;
internal class ConfigHook
{
    public static string? _configPath;

    public ConfigHook()
    {
        _configPath = Path.Combine(Environment.CurrentDirectory, "assets", "config", "config.json");
    }
    public bool GetConfig()
    {
        try
        {
            string json = File.ReadAllText(_configPath);
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            bool configEnabled = root.GetProperty("configEnabled").GetBoolean();
            return configEnabled;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error reading config file: {ex.Message}");
            return false;
        }
    }
}