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
        readonly PackageDatabaseManager pdm;
        readonly SourcePortDatabaseManager spdm;
        readonly Settings settings;
        #endregion

        #region Properties
        public PackageDownloadManager PackageDownloadManager { get; }
        public SourcePortDownloadManager SourcePortDownloadManager { get; }
        #endregion

        #region Events
        public delegate void DownloadStartedEventHandler(object sender, IProviderItem item);
        public event DownloadStartedEventHandler DownloadStarted;

        public delegate void DownloadFinishedEventHandler(object sender, IProviderItem item);
        public event DownloadFinishedEventHandler DownloadFinished;
        #endregion

        #region Constructors
        public DownloadManager(Settings settings, PackageDatabaseManager pdm, SourcePortDatabaseManager spdm)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.pdm = pdm ?? throw new ArgumentNullException(nameof(pdm));
            this.spdm = spdm ?? throw new ArgumentNullException(nameof(spdm));

            PackageDownloadManager = new PackageDownloadManager(this.settings.DownloadsPath, this.settings.PackagesPath, this.pdm);
            SourcePortDownloadManager = new SourcePortDownloadManager(this.settings.DownloadsPath, this.settings.SourcePortsPath, this.spdm);
        }
        #endregion

        #region Methods
        public async Task<string> DownloadItemAsync(IProviderItem item)
        {
            if (item is Package package)
            {
                DownloadStarted.Invoke(this, item);
                string s = await PackageDownloadManager.DownloadItemAsync(package);
                DownloadFinished.Invoke(this, item);
                return s;
            }
            else if (item is SourcePort sourcePort)
            {
                DownloadStarted.Invoke(this, item);
                string s = await SourcePortDownloadManager.DownloadItemAsync(sourcePort);
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
