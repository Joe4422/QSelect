using LibQuakePackageManager.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibQSelect
{
    public class GameManager
    {
        #region Properties
        public SourcePort LoadedSourcePort { get; protected set; } = null;
        public List<Package> LoadedPackages { get; } = new List<Package>();
        public bool GameRunning { get; protected set; } = false;
        #endregion

        #region Methods
        public void LoadSourcePort(SourcePort sourcePort)
        {
            LoadedSourcePort = sourcePort ?? throw new ArgumentNullException(nameof(sourcePort));
        }

        public void LoadPackage(Package package)
        {
            if (package is null) throw new ArgumentNullException(nameof(package));
            if (LoadedSourcePort is null) throw new Exception($"{nameof(LoadedSourcePort)} was null.");
            if (LoadedPackages.Contains(package)) return;

            string pkgPath = $"{Settings.AppSettings.PackagesPath}/{package.Id}";
            string spPath = $"{Settings.AppSettings.SourcePortsPath}/{LoadedSourcePort.Id}";

            // Create symlink into directory
            Symlink.CreateDirectory($"{spPath}/{package.Id}", pkgPath);

            // Add to list
            LoadedPackages.Add(package);
        }

        public void UnloadPackage(Package package)
        {
            if (package is null) throw new ArgumentNullException(nameof(package));
            if (LoadedSourcePort is null) throw new Exception($"{nameof(LoadedSourcePort)} was null.");
            if (!LoadedPackages.Contains(package)) return;

            string pkgPath = $"{Settings.AppSettings.SourcePortsPath}/{LoadedSourcePort.Id}/{package.Id}";

            // Delete symlink
            if (Directory.Exists(pkgPath))
            {
                Directory.Delete(pkgPath);
            }

            // Remove from list
            LoadedPackages.Remove(package);
        }

        public void UnloadAllPackages()
        {
            foreach (Package pkg in LoadedPackages.ToList())
            {
                UnloadPackage(pkg);
            }
        }

        public async Task ExecuteLoadedSourcePortAsync()
        {
            if (LoadedSourcePort is null) throw new Exception($"{nameof(LoadedSourcePort)} was null.");
            if (LoadedPackages.Count == 0) throw new Exception($"{nameof(LoadedPackages)} was empty.");
            if (LoadedSourcePort.Executable is null) throw new Exception("Cannot start source port - unknown executable.");

            StringBuilder sb = new StringBuilder();
            foreach (Package pkg in LoadedPackages)
            {
                sb.Append($"-game {pkg.Id} ");
            }

            // Get current CWD
            string cwd = Directory.GetCurrentDirectory();

            // Set CWD
            Directory.SetCurrentDirectory($"{Settings.AppSettings.SourcePortsPath}/{LoadedSourcePort.Id}/");

            // Start game, and wait until it's over
            await Task.Run(() => Process.Start(LoadedSourcePort.Executable, sb.ToString()).WaitForExit());

            // Revert CWD
            Directory.SetCurrentDirectory(cwd);
        }
        #endregion
    }
}
