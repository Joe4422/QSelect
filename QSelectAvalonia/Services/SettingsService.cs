using LibQSelect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.Services
{
    public static class SettingsService
    {
        #region Variables
        private const string settingsPath = "config.json";
        #endregion

        #region Properties
        public static Settings Settings { get; private set; }
        #endregion

        #region Methods
        public static async Task InitialiseAsync()
        {
            if (!File.Exists(settingsPath))
            {
                Settings = new();
                Settings.ConfigPath = settingsPath;
                await Settings.WriteSettingsAsync();
            }
            else
            {
                Settings = await Settings.ReadSettingsAsync(settingsPath);
            }
        }
        #endregion
    }
}
