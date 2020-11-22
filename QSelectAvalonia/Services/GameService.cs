using LibQSelect;
using LibQSelect.PackageManager.Packages;
using LibQSelect.PackageManager.SourcePorts;
using System;
using System.Collections.Generic;
using System.Text;

namespace QSelectAvalonia.Services
{
    public static class GameService
    {
        #region Properties
        public static GameLauncher Game { get; private set; } = null;
        #endregion

        #region Events
        public delegate void InitialisedEventHandler();
        public static event InitialisedEventHandler Initialised;
        #endregion

        #region Methods
        public static void Initialise()
        {
            if (Game != null) throw new Exception("Attempted to re-initialise GameService.");

            Game = new GameLauncher(SettingsService.Settings, DatabaseService.Packages, DatabaseService.SourcePorts);

            Initialised?.Invoke();
        }
        #endregion
    }
}
