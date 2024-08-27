using System;
using System.IO;

namespace StreetSovereings_.src.ModLoader
{
    internal class ModLoader
    {
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

            foreach (string statement in printStatements.Skip(1))
            {
                int endIndex = statement.IndexOf(')');
                if (endIndex != -1)
                {
                    string printArgument = statement.Substring(0, endIndex).Trim();
                    Console.WriteLine(printArgument);
                }
            }
        }

        internal static void NotLoad()
        {
            Console.WriteLine("NotLoad");
        }
    }
}
