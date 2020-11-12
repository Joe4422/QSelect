using LibQSelect;
using LibQSelect.PackageManager.Packages;
using System;
using System.Collections.Generic;
using System.Text;

namespace QSelectAvalonia.Services
{
    public static class GameService
    {
        #region Properties
        public static GameManager Game { get; private set; } = null;
        #endregion

        #region Methods
        public static void Initialise(Settings settings, PackageDatabaseManager packages)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            if (packages is null) throw new ArgumentNullException(nameof(packages));

            if (Game != null) throw new Exception("Attempted to re-initialise GameService.");

            Game = new GameManager(settings, packages);
        }
        #endregion
    }
}
