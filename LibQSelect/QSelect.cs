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

        protected IAppSettings settings;

        public QSelect(string configPath)
        {
            // Load settings
            ConfigurationBuilder<IAppSettings> builder = new ConfigurationBuilder<IAppSettings>().UseJsonFile(configPath);
            settings = builder.Build();

            // Ensure settings are written to file
            settings.BinariesDirectory = settings.BinariesDirectory;
            settings.ModsDirectory = settings.ModsDirectory;
            settings.SyncQuakeConfig = settings.SyncQuakeConfig;

            // Read config file
            JObject root = JObject.Parse(File.ReadAllText(configPath));

            // Load binaries and mods
            Binaries = Binary.LoadBinaries(settings, root);
            Mods = Mod.LoadMods(settings, root);
        }

        public void RunBinary(Binary binary, List<Mod> mods)
        {
            binary.RunBinary(settings, mods);
        }
    }
}
