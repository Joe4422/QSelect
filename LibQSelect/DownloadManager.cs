using LibQuakePackageManager.Databases;
using LibQuakePackageManager.Downloads;
using LibQuakePackageManager.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect
{
    public class DownloadManager
    {
        #region Variables
        PackageDownloadManager packageDownloadManager;
        SourcePortDownloadManager sourcePortDownloadManager;
        PackageDatabaseManager pdm;
        SourcePortDatabaseManager spdm;
        #endregion

        #region Constructors
        public DownloadManager(PackageDatabaseManager pdm, SourcePortDatabaseManager spdm)
        {
            this.pdm = pdm ?? throw new ArgumentNullException(nameof(pdm));
            this.spdm = spdm ?? throw new ArgumentNullException(nameof(spdm));

            packageDownloadManager = new PackageDownloadManager(Settings.AppSettings.DownloadsPath, Settings.AppSettings.PackagesPath);
            sourcePortDownloadManager = new SourcePortDownloadManager(Settings.AppSettings.DownloadsPath, Settings.AppSettings.SourcePortsPath);
        }
        #endregion

        #region Methods
        public async Task DownloadItemAsync(IProviderItem item)
        {
            if (item is Package package)
            {
                await packageDownloadManager.DownloadItemAsync(package);
                await pdm.SaveDatabaseAsync();
            }
            else if (item is SourcePort sourcePort)
            {
                await sourcePortDownloadManager.DownloadItemAsync(sourcePort);
                await spdm.SaveDatabaseAsync();
            }
            else
            {
                throw new ArgumentException("Unknown provider item type.");
            }
        }
        #endregion
    }
}
