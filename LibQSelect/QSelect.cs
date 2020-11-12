using LibQSelect.PackageManager.Packages;
using LibQSelect.PackageManager.SourcePorts;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibQSelect
{
    public class QSelect
    {
        #region Properties
        public PackageDatabaseManager PackageDatabaseManager { get; }
        public SourcePortDatabaseManager SourcePortDatabaseManager { get; }
        public DownloadManager DownloadManager { get; }
        public GameManager GameManager { get; }
        #endregion Properties
    }
}
