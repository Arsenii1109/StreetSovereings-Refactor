using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreetSovereings_.src.hooks
{
    internal class configHook
    {
        private readonly string configPath = "./assets/config/config.json";
        
        public bool GetConfig()
        {
            try
            {
                string json = File.ReadAllText(configPath);
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
}