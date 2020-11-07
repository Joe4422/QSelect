using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibQuakePackageManager.Providers
{
    public class LocalPackageProvider : IProvider<Package>
    {
        #region Variables
        protected string packageDirPath;
        #endregion

        #region Properties
        public List<Package> Items { get; } = new List<Package>();
        #endregion

        #region Constructors
        public LocalPackageProvider(string packageDirPath)
        {
            this.packageDirPath = packageDirPath ?? throw new ArgumentNullException(nameof(packageDirPath));
        }
        #endregion

        #region Methods
        public Package GetItem(string id)
        {
            try
            {
                return Items.First(x => x.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

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
                    Package package = new Package(id);
                    package.InstallDirectory = $"{packageDirPath}/{id}";

                    Items.Add(package);
                }
            });
        }
        #endregion Methods
    }
}
