using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LibQSelect.PackageManager.SourcePorts
{
    public class SourcePort : IRepositoryItem
    {
        #region Enums
        public enum OperatingSystem
        {
            Unknown,
            Win32,
            Win64,
            Linux32,
            Linux64,
            MacOS
        }
        #endregion

        #region Properties
        public string Id { get; }
        public ProgressToken Token { get; } = new();
        public string DownloadUrl { get; }
        public Dictionary<string, IRepositoryItem> Dependencies { get; } = new();

        /// <summary>
        /// The name of the source port.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The author(s) of the source port.
        /// </summary>
        public string Author { get; }
        /// <summary>
        /// The path of the executable relative to the base folder of the source port.
        /// </summary>
        public string Executable { get; }
        /// <summary>
        /// The OS the source port runs on.
        /// </summary>
        public OperatingSystem SupportedOS { get; }
        #endregion

        #region Constructors
        public SourcePort
        (
            string id,
            string downloadUrl,
            string name = null,
            string author = null,
            string executable = null,
            OperatingSystem os = OperatingSystem.Unknown)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DownloadUrl = downloadUrl;

            Name = name;
            Author = author;
            Executable = executable;
            SupportedOS = os;
        }
        #endregion
    }
}
