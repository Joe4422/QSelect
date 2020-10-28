using Config.Net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibQSelect
{
    public class QSelect
    {
        private Binary selectedBinary;

        public List<Binary> Binaries { get; protected set; }
        public List<Mod> Mods { get; protected set; }
        public IAppSettings Settings { get; }
        public Discord Discord { get; }
        public List<Mod> EnabledMods { get; } = new List<Mod>();
        public Binary SelectedBinary
        {
            get => selectedBinary;
            set
            {
                selectedBinary = value;
                Settings.LastBinary = selectedBinary.Directory;
            }
        }

        public QSelect(string configPath)
        {
            // Load settings
            ConfigurationBuilder<IAppSettings> builder = new ConfigurationBuilder<IAppSettings>().UseJsonFile(configPath);
            Settings = builder.Build();

            // Ensure settings are written to file
            Settings.BinariesDirectory = Settings.BinariesDirectory;
            Settings.ModsDirectory = Settings.ModsDirectory;
            Settings.SyncQuakeConfig = Settings.SyncQuakeConfig;
            Settings.EnabledMods = Settings.EnabledMods;
            Settings.LastBinary = Settings.LastBinary;

            // Read config file
            JObject root = JObject.Parse(File.ReadAllText(configPath));

            // Load binaries and mods
            Binaries = Binary.LoadBinaries(this, root);
            Mods = Mod.LoadMods(this, root);

            // Set up EnabledMods
            foreach (string s in Settings.EnabledMods.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            {
                var mod = Mods.Where((x) => x.Directory == s);
                if (mod.Count() > 0)
                {
                    EnabledMods.Add(mod.First());
                }
            }

            // Set up SelectedBinary
            var binary = Binaries.Where((x) => x.Directory == Settings.LastBinary);
            if (binary.Count() > 0)
            {
                SelectedBinary = binary.First();
            }
            else
            {
                SelectedBinary = Binaries.First();
            }

            // Set up Discord Rich Presence
            Discord = new Discord(this);
            Discord.Update();
        }

        public void RunBinary()
        {
            SelectedBinary.RunBinary(this, EnabledMods);
        }

        public void EnableMod(Mod mod)
        {
            if (EnabledMods.Contains(mod) == false)
            {
                EnabledMods.Add(mod);
                WriteLoadOrder();
            }
        }

        public void DisableMod(Mod mod)
        {
            if (EnabledMods.Contains(mod))
            {
                EnabledMods.Remove(mod);
                WriteLoadOrder();
            }
        }

        public void MoveInLoadOrder(Mod mod, int amount)
        {
            if (EnabledMods.Contains(mod))
            {
                int index = EnabledMods.IndexOf(mod);

                EnabledMods.Remove(mod);
                EnabledMods.Insert(index + amount, mod);
                WriteLoadOrder();
            }
        }

        protected void WriteLoadOrder()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Mod mod in EnabledMods)
            {
                sb.Append(mod.Directory + " ");
            }

            Settings.EnabledMods = sb.ToString();
        }
    }
}
