using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager
{
    public class InstalledPackageRepository : IRepository<Package>
    {
        #region Variables
        protected string packageDirPath;
        #endregion

        #region Properties
        public List<Package> Items { get; } = new List<Package>();
        #endregion

        #region Constructors
        public InstalledPackageRepository(string packageDirPath)
        {
            this.packageDirPath = packageDirPath ?? throw new ArgumentNullException(nameof(packageDirPath));
        }
        #endregion

        #region Methods
        public async Task RefreshAsync()
        {
            // Check directory exists
            if (!Directory.Exists(packageDirPath)) throw new DirectoryNotFoundException("Specified package directory was not found.");

            // Clear existing package list
            Items.Clear();

            // Load packages by directory name
            await Task.Run(() =>
            {
                foreach (string id in Directory.GetDirectories(packageDirPath).Select(x => Path.GetFileName(x)))
                {
                    Package package = new Package(id)
                    {
                        InstallPath = $"{packageDirPath}/{id}"
                    };

                    Items.Add(package);
                }
            });
        }
        #endregion Methods
    }
}
