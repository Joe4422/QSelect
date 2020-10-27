using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LibQSelect
{
    public class Binary
    {
        public string Directory { get; }
        public string Alias { get; set; }
        public string Executable { get; set; }

        public Binary(string directory, string alias = null, string executable = null)
        {
            Directory = directory;
            Alias = alias;
            Executable = executable;
        }

        public static List<Binary> LoadBinaries(IAppSettings settings, JObject root)
        {
            List<Binary> binaries = new List<Binary>();
            foreach (string directory in System.IO.Directory.GetDirectories(settings.BinariesDirectory).Select((x) => Path.GetFileName(x)))
            {
                Binary binary = new Binary(directory);

                if (root != null && root["BinaryMetadata"] != null && root["BinaryMetadata"][directory] != null)
                {
                    binary.Alias = (string)root["BinaryMetadata"][directory]["Alias"];
                    binary.Executable = (string)root["BinaryMetadata"][directory]["Executable"];
                }

                binaries.Add(binary);
            }

            return binaries;
        }

        public void RunBinary(IAppSettings settings, List<Mod> modList)
        {
            string cwd = System.IO.Directory.GetCurrentDirectory();
            StringBuilder argBuilder = new StringBuilder();

            // Load mods and build argument string
            foreach (Mod mod in modList)
            {
                mod.LoadMod(settings, this);
                argBuilder.Append($"-game {mod.Directory} ");
            }

            // Change current working directory to folder of binary
            System.IO.Directory.SetCurrentDirectory($"{settings.BinariesDirectory}/{Directory}");

            // Start game, and wait until it has finished
            Process p = Process.Start(Executable, argBuilder.ToString());
            while (p.HasExited == false) ;

            // Revert working directory to top level folder
            System.IO.Directory.SetCurrentDirectory(cwd);

            // Unload each mod sequentially
            modList.ForEach((mod) => mod.UnloadMod(settings, this));
        }

        public override string ToString()
        {
            if (Alias == null) return Directory;
            else return $"{Alias} ({Directory})";
        }
    }
}
