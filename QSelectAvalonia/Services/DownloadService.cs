using LibPackageManager.Managers;
using LibPackageManager.Repositories;
using LibQSelect.PackageManager.Packages;
using LibQSelect.PackageManager.SourcePorts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.Services
{
    public static class DownloadService
    {
        #region Properties
        public static PackageDownloadManager Packages { get; private set; } = null;
        public static SourcePortDownloadManager SourcePorts { get; private set; } = null;
        #endregion

        #region Methods
        public static void Initialise(string downloadsPath, string packagesPath, string sourcePortsPath)
        {
            if (downloadsPath is null) throw new ArgumentNullException(nameof(downloadsPath));
            if (packagesPath is null) throw new ArgumentNullException(nameof(packagesPath));
            if (sourcePortsPath is null) throw new ArgumentNullException(nameof(sourcePortsPath));

            if (Packages != null || SourcePorts != null) throw new Exception("Attempted to re-initialise DownloadService.");

            Packages = new PackageDownloadManager(downloadsPath, packagesPath);
            SourcePorts = new SourcePortDownloadManager(downloadsPath, sourcePortsPath);
        }
        #endregion
    }
}
