using LibQuakePackageManager.Databases;
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
        #region Variables
        protected PackageDatabaseManager pdm;
        protected Settings settings;
        #endregion

        #region Properties
        public SourcePort LoadedSourcePort { get; protected set; } = null;
        public List<Package> LoadedPackages { get; } = new List<Package>();
        public bool GameRunning { get; protected set; } = false;
        #endregion

        #region Constructors
        public GameManager(Settings settings, PackageDatabaseManager pdm)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.pdm = pdm ?? throw new ArgumentNullException(nameof(pdm));
        }
        #endregion

        #region Methods
        public void LoadSourcePort(SourcePort sourcePort)
        {
            LoadedSourcePort = sourcePort ?? throw new ArgumentNullException(nameof(sourcePort));
        }

        public async Task<string> LoadPackageAsync(Package package)
        {
            if (package is null) throw new ArgumentNullException(nameof(package));
            if (LoadedSourcePort is null) throw new Exception($"{nameof(LoadedSourcePort)} was null.");

            List<Task> loadTasks = new List<Task>();
            List<Package> dependencies = new List<Package>();

            if (package.Dependencies != null)
            {
                foreach (string dpId in package.Dependencies)
                {
                    Package dpPkg = pdm[dpId];

                    if (dpPkg is null) return dpId;
                    else dependencies.Add(dpPkg);
                }
            }

            loadTasks.Add(LoadSinglePackageAsync(package));

            foreach (Package dpPkg in dependencies)
            {
                loadTasks.Add(LoadPackageAsync(dpPkg));
            }

            await Task.WhenAll(loadTasks);

            return null;
        }

        protected async Task LoadSinglePackageAsync(Package package)
        {
            if (LoadedPackages.Contains(package)) return;

            await Task.Run(() =>
            {
                string pkgPath = $"{settings.PackagesPath}/{package.Id}";
                string spPath = $"{settings.SourcePortsPath}/{LoadedSourcePort.Id}";

                // Create symlink into directory
                Symlink.CreateDirectory($"{spPath}/{package.Id}", pkgPath);
            });

            // Add to list
            LoadedPackages.Add(package);
        }

        public async Task<string> UnloadPackageAsync(Package package)
        {
            if (package is null) throw new ArgumentNullException(nameof(package));
            if (LoadedSourcePort is null) throw new Exception($"{nameof(LoadedSourcePort)} was null.");
            if (!LoadedPackages.Contains(package)) return null;

            List<Package> dependencies = new List<Package>();
            if (package.Dependencies != null)
            {
                foreach (string dpId in package.Dependencies)
                {
                    Package dpPkg = pdm[dpId];

                    if (dpPkg != null) dependencies.Add(dpPkg);
                    // Honestly we don't really care about missing dependencies here
                }
            }

            if (await UnloadSinglePackageAsync(package) == false)
            {
                return package.Id;
            }

            foreach (Package dpPkg in dependencies)
            {
                string result = await UnloadPackageAsync(dpPkg);

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
                    if (loadedPkg.Dependencies != null && loadedPkg.Dependencies.Contains(package.Id)) return false;
                }
            }

            await Task.Run(() =>
            {
                string pkgPath = $"{settings.SourcePortsPath}/{LoadedSourcePort.Id}/{package.Id}";

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

        public void UnloadAllPackages()
        {
            foreach (Package pkg in LoadedPackages.ToList())
            {
                UnloadSinglePackageAsync(pkg, true).Wait();
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
            Directory.SetCurrentDirectory($"{settings.SourcePortsPath}/{LoadedSourcePort.Id}/");

            // Start game, and wait until it's over
            await Task.Run(() => Process.Start(LoadedSourcePort.Executable, sb.ToString()).WaitForExit());

            // Revert CWD
            Directory.SetCurrentDirectory(cwd);
        }
        #endregion
    }
}
