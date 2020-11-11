using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager
{
    public class InstalledSourcePortRepository : IRepository<SourcePort>
    {
        #region Variables
        protected string sourcePortDirPath;
        #endregion

        #region Properties
        public List<SourcePort> Items { get; } = new List<SourcePort>();
        #endregion

        #region Constructors
        public InstalledSourcePortRepository(string sourcePortDirPath)
        {
            this.sourcePortDirPath = sourcePortDirPath ?? throw new ArgumentNullException(nameof(sourcePortDirPath));
        }
        #endregion Constructors

        #region Methods
        public async Task RefreshAsync()
        {
            // Check directory exists
            if (!Directory.Exists(sourcePortDirPath)) throw new DirectoryNotFoundException("Specified source port directory was not found.");

            // Clear existing source port list
            Items.Clear();

            // Load source ports by directory name
            await Task.Run(() =>
            {
                foreach (string id in Directory.GetDirectories(sourcePortDirPath).Select(x => Path.GetFileName(x)))
                {
                    SourcePort.OperatingSystem os = id.Split("-", StringSplitOptions.RemoveEmptyEntries).Last() switch
                    {
                        "win32" => SourcePort.OperatingSystem.Win32,
                        "win64" => SourcePort.OperatingSystem.Win64,
                        "linux32" => SourcePort.OperatingSystem.Linux32,
                        "linux64" => SourcePort.OperatingSystem.Linux64,
                        "macos" => SourcePort.OperatingSystem.MacOS,
                        _ => SourcePort.OperatingSystem.Unknown
                    };
                    SourcePort sourcePort = new SourcePort(id, os: os)
                    {
                        InstallPath = $"{sourcePortDirPath}/{id}"
                    };

                    Items.Add(sourcePort);
                }
            });
        }
        #endregion
    }
}
