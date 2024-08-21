using System;
using System.IO;

namespace StreetSovereigns.src.hooks
{
    internal class ModLoaderHook
    {
        internal static bool CheckModLoader()
        {
            return File.Exists("./SSModLoader.exe");
        }
    }
}