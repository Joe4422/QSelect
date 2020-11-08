using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibQSelect
{
    public class Settings
    {
        public static Settings AppSettings { get; protected set; } = new Settings();

        public string DownloadsPath { get; set; } = "Downloads";
        public string PackagesPath { get; set; } = "Packages";
        public string SourcePortsPath { get; set; } = "SourcePorts";

        protected Settings() { }

        public static void LoadSettings(string settingsFile)
        {
            if (File.Exists(settingsFile))
            {
                AppSettings = JsonConvert.DeserializeObject<Settings>(settingsFile);
            }
            else
            {
                Settings s = new Settings();
                AppSettings = s;

                SaveSettings(settingsFile);
            }
        }

        public static void SaveSettings(string settingsFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsFile));
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(AppSettings));
        }
    }
}
