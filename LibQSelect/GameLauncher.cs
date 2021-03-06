﻿using LibQSelect.PackageManager.Packages;
using LibQSelect.PackageManager.SourcePorts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibQSelect
{
    public class GameLauncher : INotifyPropertyChanged
    {
        #region Variables
        protected PackageDatabaseManager pdm;
        protected Settings settings;
        #endregion

        #region Properties
        private SourcePort loadedSourcePort = null; public SourcePort LoadedSourcePort
        {
            get => loadedSourcePort;
            set
            {
                loadedSourcePort = value;
                PropertyChanged?.Invoke(this, new(nameof(LoadedSourcePort)));
            }
        }
        public List<Package> LoadedPackages { get; } = new List<Package>();
        private bool gameRunning; public bool GameRunning
        {
            get => gameRunning;
            protected set
            {
                gameRunning = value;
                PropertyChanged?.Invoke(this, new(nameof(GameRunning)));
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public GameLauncher(Settings settings, PackageDatabaseManager pdm, SourcePortDatabaseManager spdm)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.pdm = pdm ?? throw new ArgumentNullException(nameof(pdm));

            if (settings.LastPackageID != null)
            {
                Package pkg = pdm[settings.LastPackageID];
                if (pkg != null)
                {
                    LoadPackageAsync(pkg).ConfigureAwait(false);
                }
            }

            if (settings.LastSourcePortID != null)
            {
                SourcePort sp = spdm[settings.LastSourcePortID];
                if (sp != null)
                {
                    LoadSourcePort(sp);
                }
            }
        }
        #endregion

        #region Methods
        public void LoadSourcePort(SourcePort sourcePort)
        {
            LoadedSourcePort = sourcePort ?? throw new ArgumentNullException(nameof(sourcePort));
            settings.LastSourcePortID = sourcePort.Id;
        }

        public async Task<string> LoadPackageAsync(Package package)
        {
            if (LoadedSourcePort is null) throw new Exception($"{nameof(LoadedSourcePort)} was null.");

            string s = await LoadPackagesAsync(package.Id, package);

            settings.LastPackageID = package.Id;

            return s;
        }

        protected async Task<string> LoadPackagesAsync(string id, Package package)
        {
            List<Task<string>> loadTasks = new();

            foreach (string key in package.Dependencies.Keys)
            {
                loadTasks.Add(LoadPackagesAsync(key, package.Dependencies[key] as Package));
            }
            await Task.WhenAll(loadTasks);

            if (await LoadSinglePackageAsync(package) == false)
            {
                return id;
            }


            return null;
        }

        protected async Task<bool> LoadSinglePackageAsync(Package package)
        {
            if (LoadedPackages.Contains(package)) return true;
            else if (package is null) return false;

            await Task.Run(() =>
            {
                string pkgPath = $"{settings.PackagesPath}/{package.Id}/";
                string spPath = $"{settings.SourcePortsPath}/{LoadedSourcePort.Id}/";

                // Create symlink into directory
                Symlink.CreateDirectory($"{spPath}/{package.Id}", pkgPath);
            });

            // Add to list
            LoadedPackages.Add(package);

            return true;
        }

        public async Task<string> UnloadPackageAsync(Package package)
        {
            if (package is null) throw new ArgumentNullException(nameof(package));
            if (LoadedSourcePort is null) throw new Exception($"{nameof(LoadedSourcePort)} was null.");
            if (!LoadedPackages.Contains(package)) return null;

            if (await UnloadSinglePackageAsync(package) == false)
            {
                return package.Id;
            }

            foreach (string key in package.Dependencies.Keys)
            {
                if (package.Dependencies[key] == null) continue;

                string result = await UnloadPackageAsync(package.Dependencies[key] as Package);

                if (result != null) return result;
            }

            return null;
        }

        protected async Task<bool> UnloadSinglePackageAsync(Package package, bool force = false)
        {
            // Check no remaining packages are dependent on me
            if (!force)
            {
                foreach (Package loadedPkg in LoadedPackages)
                {
                    if (loadedPkg.Dependencies != null && loadedPkg.Dependencies.Keys.Contains(package.Id)) return false;
                }
            }

            await Task.Run(() =>
            {
                string pkgPath = $"{settings.SourcePortsPath}/{LoadedSourcePort.Id}/{package.Id}/";

                // Delete symlink
                if (Directory.Exists(pkgPath))
                {
                    Directory.Delete(pkgPath);
                }
            });

            // Remove from list
            LoadedPackages.Remove(package);

            return true;
        }

        public async Task UnloadAllPackagesAsync()
        {
            foreach (Package pkg in LoadedPackages.ToList())
            {
                await UnloadSinglePackageAsync(pkg, true);
            }
        }

        public async Task ExecuteLoadedSourcePortAsync(string args = "+map start")
        {
            if (LoadedSourcePort is null) throw new Exception($"{nameof(LoadedSourcePort)} was null.");
            if (LoadedPackages.Count == 0) throw new Exception($"{nameof(LoadedPackages)} was empty.");
            if (LoadedSourcePort.Executable is null) throw new Exception("Cannot start source port - unknown executable.");

            StringBuilder sb = new StringBuilder();
            foreach (Package pkg in LoadedPackages)
            {
                sb.Append($"-game {pkg.Id} ");
            }

            if (args != null)
            {
                sb.Append(args);
            }

            // Get current CWD
            string cwd = Directory.GetCurrentDirectory();

            // Set CWD
            Directory.SetCurrentDirectory($"{settings.SourcePortsPath}/{LoadedSourcePort.Id}/");

            // Start game, and wait until it's over
            await Task.Run(() => Process.Start(LoadedSourcePort.Executable, sb.ToString()).WaitForExit());

            // Revert CWD
            Directory.SetCurrentDirectory(cwd);
        }
        #endregion
    }
}
