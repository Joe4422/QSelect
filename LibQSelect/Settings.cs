using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect
{
    public record Settings
    {
        public string ConfigPath { get; set; }
        private string downloadsPath = "Downloads"; public string DownloadsPath
        {
            get => downloadsPath;
            set
            {
                downloadsPath = value;
                WriteSettingsAsync().ConfigureAwait(false);
            }
        }
        private string packagesPath = "Packages"; public string PackagesPath
        {
            get => packagesPath;
            set
            {
                packagesPath = value;
                WriteSettingsAsync().ConfigureAwait(false);
            }
        }
        private string sourcePortsPath = "SourcePorts"; public string SourcePortsPath
        {
            get => sourcePortsPath;
            set
            {
                sourcePortsPath = value;
                WriteSettingsAsync().ConfigureAwait(false);
            }
        }
        private string lastPackageId = null;  public string LastPackageID
        {
            get => lastPackageId;
            set
            {
                lastPackageId = value;
                WriteSettingsAsync().ConfigureAwait(false);
            }
        }
        private string lastSourcePortId = null; public string LastSourcePortID
        {
            get => lastSourcePortId;
            set
            {
                lastSourcePortId = value;
                WriteSettingsAsync().ConfigureAwait(false);
            }
        }

        public static async Task<Settings> ReadSettingsAsync(string configPath)
        {
            Settings s = JsonConvert.DeserializeObject<Settings>(await File.ReadAllTextAsync(configPath));
            s.ConfigPath = configPath;
            return s;
        }

        public async Task WriteSettingsAsync()
        {
            await File.WriteAllTextAsync(ConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
