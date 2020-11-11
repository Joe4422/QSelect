using LibPackageManager.Repositories;
using LibQSelect.PackageManager;
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
        public PackageDownloadManager PackageManager { get; }
        public SourcePortDownloadManager SourcePortManager { get; }
        #endregion

        #region Events
        public delegate void DownloadStartedEventHandler(object sender, IRepositoryItem item);
        public event DownloadStartedEventHandler DownloadStarted;

        public delegate void DownloadFinishedEventHandler(object sender, IRepositoryItem item);
        public event DownloadFinishedEventHandler DownloadFinished;
        #endregion

        #region Constructors
        public DownloadManager(Settings settings, PackageDatabaseManager pdm, SourcePortDatabaseManager spdm)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.pdm = pdm ?? throw new ArgumentNullException(nameof(pdm));
            this.spdm = spdm ?? throw new ArgumentNullException(nameof(spdm));

            PackageManager = new PackageDownloadManager(this.settings.DownloadsPath, this.settings.PackagesPath);
            SourcePortManager = new SourcePortDownloadManager(this.settings.DownloadsPath, this.settings.SourcePortsPath);
        }
        #endregion

        #region Methods
        public async Task<bool> DownloadItemAsync(IRepositoryItem item)
        {
            if (item is Package package)
            {
                DownloadStarted?.Invoke(this, item);
                bool result = await PackageManager.GetItemAsync(package);
                DownloadFinished?.Invoke(this, item);
                return result;
            }
            else if (item is SourcePort sourcePort)
            {
                DownloadStarted?.Invoke(this, item);
                bool result = await SourcePortManager.GetItemAsync(sourcePort);
                DownloadFinished?.Invoke(this, item);
                return result;
            }
            else
            {
                throw new ArgumentException("Unknown provider item type.");
            }
        }
        #endregion
    }
}
