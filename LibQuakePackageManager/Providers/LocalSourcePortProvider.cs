using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibQuakePackageManager.Providers
{
    public class LocalSourcePortProvider : IProvider<SourcePort>
    {
        #region Variables
        protected string sourcePortDirPath;
        #endregion

        #region Properties
        public List<SourcePort> Items { get; } = new List<SourcePort>();
        #endregion

        #region Constructors
        public LocalSourcePortProvider(string sourcePortDirPath)
        {
            this.sourcePortDirPath = sourcePortDirPath ?? throw new ArgumentNullException(nameof(sourcePortDirPath));
        }
        #endregion Constructors

        #region Methods
        public SourcePort GetItem(string id)
        {
            try
            {
                return Items.First(x => x.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task RefreshAsync()
        {
            // Check directory exists
            if (!Directory.Exists(sourcePortDirPath)) throw new DirectoryNotFoundException("Specified source port directory was not found.");

            // Clear existing source port list
            Items.Clear();

            // Load packages by directory name
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
                    SourcePort sourcePort = new SourcePort(id, os: os);
                    sourcePort.InstallDirectory = $"{sourcePortDirPath}/{id}";

                    Items.Add(sourcePort);
                }
            });
        }
        #endregion
    }
}
