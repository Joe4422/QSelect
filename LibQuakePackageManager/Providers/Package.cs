using System;
using System.Collections.Generic;
using System.Text;

namespace LibQuakePackageManager.Providers
{
    public class Package : IProviderItem
    {
        #region Properties
        /// <summary>
        /// The package's unique identifier.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// The MD5 checksum value of the package.
        /// </summary>
        public string MD5Checksum { get; }
        /// <summary>
        /// The directory, relative to the Quake main folder, that the package should be unzipped to.
        /// </summary>
        public string UnzipDirectory { get; }
        /// <summary>
        /// The URL at which the package exists.
        /// </summary>
        public string DownloadUrl { get; }
        /// <summary>
        /// List of attributes of this package.
        /// </summary>
        public Dictionary<string, string> Attributes { get; }

        /// <summary>
        /// The directory path where this package is installed.
        /// </summary>
        public string InstallDirectory { get; set; } = null;
        /// <summary>
        /// True if this package has been downloaded, false otherwise.
        /// </summary>
        public bool IsDownloaded => InstallDirectory != null;
        #endregion

        #region Constructors
        public Package(string id, Dictionary<string, string> attributes = default, string mD5Checksum = null, string unzipDirectory = null, string downloadUrl = null)
        {
            Id = id;
            Attributes = attributes;
            MD5Checksum = mD5Checksum;
            UnzipDirectory = unzipDirectory;
            DownloadUrl = downloadUrl;
        }
        #endregion
    }
}
