using Config.Net;
using LibQuaddicted;
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
        private SourcePortOld selectedSourcePort;

        public List<SourcePortOld> SourcePorts { get; protected set; }
        public List<Mod> Mods { get; protected set; }
        public IAppSettings Settings { get; }
        public DiscordRichPresenceHandler Discord { get; }
        public List<Mod> EnabledMods { get; } = new List<Mod>();
        public SourcePortOld SelectedSourcePort
        {
            get => selectedSourcePort;
            set
            {
                selectedSourcePort = value;
                Settings.LastSourcePort = selectedSourcePort.Directory;
            }
        }
        public ModDatabase ModDatabase { get; }
        public DownloadDatabase DownloadDatabase { get; }

        public QSelect(string configPath)
        {
            // Load settings
            ConfigurationBuilder<IAppSettings> builder = new ConfigurationBuilder<IAppSettings>().UseJsonFile(configPath);
            Settings = builder.Build();

            // Ensure settings are written to file
            Settings.SourcePortsDirectory = Settings.SourcePortsDirectory;
            Settings.ModsDirectory = Settings.ModsDirectory;
            Settings.SyncQuakeConfig = Settings.SyncQuakeConfig;
            Settings.EnabledMods = Settings.EnabledMods;
            Settings.LastSourcePort = Settings.LastSourcePort;

            // Read config file
            JObject root = JObject.Parse(File.ReadAllText(configPath));

            // Load source ports and mods
            SourcePorts = SourcePortOld.LoadSourcePorts(this, root);
            Mods = Mod.LoadMods(this, root);

            // Set up EnabledMods
            foreach (string s in Settings.EnabledMods.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            {
                var mod = Mods.Where((x) => x.Id == s);
                if (mod.Count() > 0)
                {
                    EnabledMods.Add(mod.First());
                }
            }

            // Set up SelectedSourcePort
            var sourceport = SourcePorts.Where((x) => x.Directory == Settings.LastSourcePort);
            if (sourceport.Count() > 0)
            {
                SelectedSourcePort = sourceport.First();
            }
            else
            {
                SelectedSourcePort = SourcePorts.First();
            }

            // Set up Discord Rich Presence
            Discord = new DiscordRichPresenceHandler(this);
            Discord.SetActiveMod();
        }

        public void RunSourcePort()
        {
            SelectedSourcePort.RunSourcePort(this, EnabledMods);
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
                sb.Append(mod.Id + " ");
            }

            Settings.EnabledMods = sb.ToString();
        }
    }
}
