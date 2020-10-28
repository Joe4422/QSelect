using Config.Net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibQSelect
{
    public class QSelect
    {
        public List<Binary> Binaries { get; protected set; }
        public List<Mod> Mods { get; protected set; }
        public IAppSettings Settings { get; }
        public Discord Discord { get; }

        public QSelect(string configPath)
        {
            // Load settings
            ConfigurationBuilder<IAppSettings> builder = new ConfigurationBuilder<IAppSettings>().UseJsonFile(configPath);
            Settings = builder.Build();

            // Ensure settings are written to file
            Settings.BinariesDirectory = Settings.BinariesDirectory;
            Settings.ModsDirectory = Settings.ModsDirectory;
            Settings.SyncQuakeConfig = Settings.SyncQuakeConfig;

            // Read config file
            JObject root = JObject.Parse(File.ReadAllText(configPath));

            // Load binaries and mods
            Binaries = Binary.LoadBinaries(this, root);
            Mods = Mod.LoadMods(this, root);

            // Set up Discord Rich Presence
            Discord = new Discord(this);
            Discord.Update();
        }

        public void RunBinary(Binary binary, List<Mod> mods)
        {
            binary.RunBinary(this, mods);
        }
    }
}
