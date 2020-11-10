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
        string DownloadUrl { get; }
        /// <summary>
        /// The directory path where this package is installed.
        /// </summary>
        string InstallDirectory { get; set; }
        /// <summary>
        /// True if this package has been downloaded, false otherwise.
        /// </summary>
        bool IsDownloaded => InstallDirectory != null;
        /// <summary>
        /// List of item IDs that this item relies on.
        /// </summary>
        Dictionary<string, IProviderItem> Dependencies { get; }
    }
}
