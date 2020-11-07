using System;
using System.Collections.Generic;
using System.Text;

namespace LibQuakePackageManager.Providers
{
    public interface IProviderItem
    {
        /// <summary>
        /// The item's unique identifier.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// The URL at which the package exists.
        /// </summary>
        public string DownloadUrl { get; }
        /// <summary>
        /// The directory path where this package is installed.
        /// </summary>
        public string InstallDirectory { get; set; }
        /// <summary>
        /// True if this package has been downloaded, false otherwise.
        /// </summary>
        public bool IsDownloaded => InstallDirectory != null;
    }
}
