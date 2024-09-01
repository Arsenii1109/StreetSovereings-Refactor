using System;
using System.IO;
using StreetSovereings_.src;
using OpenTK.Mathematics;

namespace StreetSovereings_.src.ModLoader
{
    internal class ModLoader
    {
        private static Renderer.Game? _gameInstance;

        public static void Initialize(Renderer.Game game)
        {
            _gameInstance = game;
        }

        internal static void Load()
        {
            Console.WriteLine("Checking mods libraries...");

            string modsDirectory = "./mods/";

            if (Directory.Exists(modsDirectory))
            {
                string[] files = Directory.GetFiles(modsDirectory)
                    .Where(file => file.EndsWith(".ssmod")).ToArray();

                foreach (string file in files)
                {
                    ProcessFile(file);
                }
            }
            else
            {
                Console.WriteLine($"Directory '{modsDirectory}' does not exist.\nSkipping Error.");
            }
        }

        static void ProcessFile(string file)
        {
            string fileContent = File.ReadAllText(file);

            string[] printStatements = fileContent.Split(new[] { "print(" }, StringSplitOptions.None);
            string[] addPlaneStatements = fileContent.Split(new[] { "addPlane(" }, StringSplitOptions.None);
            string[] addCubeStatements = fileContent.Split(new[] { "addCube(" }, StringSplitOptions.None);

            foreach (string statement in printStatements.Skip(1))
            {
                int endIndex = statement.IndexOf(')');
                if (endIndex != -1)
                {
                    string printArgument = statement.Substring(0, endIndex).Trim();
                    Console.WriteLine($"{file}: {printArgument}");
                }
            }

            foreach (string statement in addPlaneStatements.Skip(1))
            {
                int endIndex = statement.IndexOf(')');
                if (endIndex != -1)
                {
                    string addPlaneArgument = statement.Substring(0, endIndex).Trim();
                    addSublity(addPlaneArgument);
                }
            }

            foreach (string statement in addCubeStatements.Skip(1))
            {
                int endIndex = statement.IndexOf(')');
                if (endIndex != -1)
                {
                    string addCubeArgument = statement.Substring(0, endIndex).Trim();
                    addCube(addCubeArgument);
                }
            }
        }

        static void addSublity(string argument)
        {
            Console.WriteLine($"Adding plane with argument: {argument}");
            var parameters = argument.Split(',');
            if (parameters.Length == 9)
            {
                float x = float.Parse(parameters[0]);
                float y = float.Parse(parameters[1]);
                float z = float.Parse(parameters[2]);
                float sizeX = float.Parse(parameters[3]);
                float sizeZ = float.Parse(parameters[4]);
                Vector4 rgba = new Vector4(
                    float.Parse(parameters[5]),
                    float.Parse(parameters[6]),
                    float.Parse(parameters[7]),
                    float.Parse(parameters[8])
                );

                _gameInstance.AddPlane(x, y, z, sizeX, sizeZ, rgba);
            }
            else
            {
                Console.WriteLine("Invalid argument format for AddPlane.");
            }
        }

        static void addCube(string argument)
        {
            Console.WriteLine($"Adding cube with argument: {argument}");
            var parameters = argument.Split(',');
            if (parameters.Length == 8)
            {
                float x = float.Parse(parameters[0]);
                float y = float.Parse(parameters[1]);
                float z = float.Parse(parameters[2]);
                Vector4 rgba = new Vector4(
                    float.Parse(parameters[3]),
                    float.Parse(parameters[4]),
                    float.Parse(parameters[5]),
                    float.Parse(parameters[6])
                );
                float mass = float.Parse(parameters[7]);

                _gameInstance.AddCube(x, y, z, rgba, mass);
            }
            else
            {
                Console.WriteLine("Invalid argument format for AddCube.");
            }
        }

        internal static void NotLoad()
        {
            Console.WriteLine("NotLoad");
        }
    }
}