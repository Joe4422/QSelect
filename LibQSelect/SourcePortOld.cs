using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LibQSelect
{
    public class SourcePortOld
    {
        public string Directory { get; }
        public string Alias { get; set; }
        public string Executable { get; set; }

        public List<Mod> ReadiedMods { get; }

        public SourcePortOld(string directory, string alias = null, string executable = null)
        {
            Directory = directory;
            Alias = alias;
            Executable = executable;

            if (alias == null) Alias = Directory;

            ReadiedMods = new List<Mod>();
        }

        public static List<SourcePortOld> LoadSourcePorts(QSelect select, JObject root)
        {
            List<SourcePortOld> sourceports = new List<SourcePortOld>();
            foreach (string directory in System.IO.Directory.GetDirectories(select.Settings.SourcePortsDirectory).Select((x) => Path.GetFileName(x)))
            {
                SourcePortOld sourceport = new SourcePortOld(directory);

                if (root != null && root["SourcePortMetadata"] != null && root["SourcePortMetadata"][directory] != null)
                {
                    sourceport.Alias = (string)root["SourcePortMetadata"][directory]["Alias"];
                    sourceport.Executable = (string)root["SourcePortMetadata"][directory]["Executable"];
                }

                sourceports.Add(sourceport);
            }

            return sourceports;
        }

        public void RunSourcePort(QSelect select, List<Mod> modList)
        {
            string cwd = System.IO.Directory.GetCurrentDirectory();
            StringBuilder argBuilder = new StringBuilder();

            // Load mods and build argument string
            foreach (Mod mod in modList)
            {
                mod.ReadyMod(select.Settings, this);
                argBuilder.Append($"-game {mod.Id} ");
            }

            // Append "-condebug"
            argBuilder.Append("-condebug");

            // Change current working directory to folder of binary
            System.IO.Directory.SetCurrentDirectory($"{select.Settings.SourcePortsDirectory}/{Directory}");

            // Update Discord Rich Presence
            select.Discord.Update(this, modList.Last());

            // Start game, and wait until it has finished
            Process p = Process.Start(Executable, argBuilder.ToString());
            while (p.HasExited == false) ;

            // Revert working directory to top level folder
            System.IO.Directory.SetCurrentDirectory(cwd);

            // Unload each mod sequentially
            modList.ForEach((mod) => mod.UnreadyMod(select.Settings, this));

            // Update Discord Rich Presence
            select.Discord.SetActiveMod();
        }

        public override string ToString()
        {
            if (Alias == Directory) return Directory;
            else return $"{Alias} ({Directory})";
        }
    }
}
