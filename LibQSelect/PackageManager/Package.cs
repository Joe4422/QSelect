using LibPackageManager.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibQSelect.PackageManager
{
    public class Package : IDependentRepositoryItem
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
        /// True if this package has been downloaded, false otherwise.
        /// </summary>
        public bool IsDownloaded => InstallPath != null;

        public Dictionary<string, IDependentRepositoryItem> Dependencies { get; }

        public string InstallPath { get; set; }
        #endregion

        #region Constructors
        public Package(string id, Dictionary<string, string> attributes = default, string mD5Checksum = null, string unzipDirectory = null, string downloadUrl = null, Dictionary<string, IDependentRepositoryItem> dependencies = null)
        {
            Id = id;
            Attributes = attributes;
            MD5Checksum = mD5Checksum;
            UnzipDirectory = unzipDirectory;
            DownloadUrl = downloadUrl;
            Dependencies = dependencies ?? new();
        }
        #endregion

        #region Methods
        public string GetAttribute(string key)
        {
            bool success = Attributes.TryGetValue(key, out string value);

            if (success) return value;
            else return null;
        }

        public bool HasAttribute(string key) => Attributes.ContainsKey(key);
        #endregion
    }
}
