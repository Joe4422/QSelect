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
        readonly PackageDownloadManager packageDownloadManager;
        readonly SourcePortDownloadManager sourcePortDownloadManager;
        readonly PackageDatabaseManager pdm;
        readonly SourcePortDatabaseManager spdm;
        #endregion

        #region Events
        public delegate void DownloadStartedEventHandler(object sender, IProviderItem item);
        public event DownloadStartedEventHandler DownloadStarted;

        public delegate void DownloadFinishedEventHandler(object sender, IProviderItem item);
        public event DownloadFinishedEventHandler DownloadFinished;
        #endregion

        #region Constructors
        public DownloadManager(PackageDatabaseManager pdm, SourcePortDatabaseManager spdm)
        {
            this.pdm = pdm ?? throw new ArgumentNullException(nameof(pdm));
            this.spdm = spdm ?? throw new ArgumentNullException(nameof(spdm));

            packageDownloadManager = new PackageDownloadManager(Settings.AppSettings.DownloadsPath, Settings.AppSettings.PackagesPath, this.pdm);
            sourcePortDownloadManager = new SourcePortDownloadManager(Settings.AppSettings.DownloadsPath, Settings.AppSettings.SourcePortsPath, this.spdm);
        }
        #endregion

        #region Methods
        public async Task<string> DownloadItemAsync(IProviderItem item)
        {
            if (item is Package package)
            {
                DownloadStarted.Invoke(this, item);
                string s = await packageDownloadManager.DownloadItemAsync(package);
                await pdm.SaveDatabaseAsync();
                DownloadFinished.Invoke(this, item);
                return s;
            }
            else if (item is SourcePort sourcePort)
            {
                DownloadStarted.Invoke(this, item);
                string s = await sourcePortDownloadManager.DownloadItemAsync(sourcePort);
                await spdm.SaveDatabaseAsync();
                DownloadFinished.Invoke(this, item);
                return s;
            }
            else
            {
                throw new ArgumentException("Unknown provider item type.");
            }
        }
        #endregion
    }
}
