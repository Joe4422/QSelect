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

            if (alias == null) Alias = Directory;
        }

        public static List<Binary> LoadBinaries(QSelect select, JObject root)
        {
            List<Binary> binaries = new List<Binary>();
            foreach (string directory in System.IO.Directory.GetDirectories(select.Settings.BinariesDirectory).Select((x) => Path.GetFileName(x)))
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

        public void RunBinary(QSelect select, List<Mod> modList)
        {
            string cwd = System.IO.Directory.GetCurrentDirectory();
            StringBuilder argBuilder = new StringBuilder();

            // Load mods and build argument string
            foreach (Mod mod in modList)
            {
                mod.LoadMod(select, this);
                argBuilder.Append($"-game {mod.Directory} ");
            }

            // Append "-condebug"
            argBuilder.Append("-condebug");

            // Change current working directory to folder of binary
            System.IO.Directory.SetCurrentDirectory($"{select.Settings.BinariesDirectory}/{Directory}");

            // Update Discord Rich Presence
            select.Discord.Update(this, modList.Last());

            // Start game, and wait until it has finished
            Process p = Process.Start(Executable, argBuilder.ToString());
            while (p.HasExited == false) ;

            // Revert working directory to top level folder
            System.IO.Directory.SetCurrentDirectory(cwd);

            // Unload each mod sequentially
            modList.ForEach((mod) => mod.UnloadMod(select.Settings, this));

            // Update Discord Rich Presence
            select.Discord.Update();
        }

        public override string ToString()
        {
            if (Alias == Directory) return Directory;
            else return $"{Alias} ({Directory})";
        }
    }
}
