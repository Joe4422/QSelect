using LibPackageManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibQSelect.PackageManager.SourcePorts
{
    public class BuiltInSourcePortRepository : IRepository<SourcePort>
    {
        #region Properties
        public List<SourcePort> Items { get; }
        public RepositoryDataSource DataSource => RepositoryDataSource.Local;
        #endregion

        #region Constructors
        public BuiltInSourcePortRepository()
        {
            Items = new List<SourcePort>()
            {
                // Quakespasm
                new SourcePort // Win32
                (
                    id: "quakespasm-win32",
                    downloadUrl: null,
                    name: "Quakespasm",
                    author: "Ozkan Sezer",
                    executable: "quakespasm.exe",
                    os: SourcePort.OperatingSystem.Win32
                ),
                new SourcePort // Win64
                (
                    id: "quakespasm-win64",
                    downloadUrl: null,
                    name: "Quakespasm",
                    author: "Ozkan Sezer",
                    executable: "quakespasm.exe",
                    os: SourcePort.OperatingSystem.Win64
                ),
                new SourcePort // Linux32
                (
                    id: "quakespasm-linux32",
                    downloadUrl: null,
                    name: "Quakespasm",
                    author: "Ozkan Sezer",
                    executable: "quakespasm",
                    os: SourcePort.OperatingSystem.Linux32
                ),
                new SourcePort // Linux64
                (
                    id: "quakespasm-linux64",
                    downloadUrl: null,
                    name: "Quakespasm",
                    author: "Ozkan Sezer",
                    executable: "quakespasm",
                    os: SourcePort.OperatingSystem.Linux64
                ),
                // Quakespasm Spiked
                new SourcePort // Win32
                (
                    id: "quakespasm-spiked-win32",
                    downloadUrl: "https://fte.triptohell.info/moodles/qss/quakespasm_spiked_win32.zip",
                    name: "Quakespasm Spiked",
                    author: "Shpoike",
                    executable: "quakespasm-spiked-win32.exe",
                    os: SourcePort.OperatingSystem.Win32
                ),
                new SourcePort // Win64
                (
                    id: "quakespasm-spiked-win64",
                    downloadUrl: "https://fte.triptohell.info/moodles/qss/quakespasm_spiked_win64.zip",
                    name: "Quakespasm Spiked",
                    author: "Shpoike",
                    executable: "quakespasm-spiked-win64.exe",
                    os: SourcePort.OperatingSystem.Win64
                ),
                new SourcePort // Linux64
                (
                    id: "quakespasm-spiked-linux64",
                    downloadUrl: "https://fte.triptohell.info/moodles/qss/quakespasm_spiked_linux64.zip",
                    name: "Quakespasm Spiked",
                    author: "Shpoike",
                    executable: "quakespasm-spiked-linux64",
                    os: SourcePort.OperatingSystem.Linux64
                ),
                // DarkPlaces
                new SourcePort // Win32
                (
                    id: "darkplaces-win32",
                    downloadUrl: "https://icculus.org/twilight/darkplaces/files/darkplacesenginewindowsonly20140513.zip",
                    name: "DarkPlaces",
                    author: "LadyHavoc",
                    executable: "darkplaces-sdl.exe",
                    os: SourcePort.OperatingSystem.Win32
                ),
                new SourcePort // Win64
                (
                    id: "darkplaces-win64",
                    downloadUrl: "https://icculus.org/twilight/darkplaces/files/darkplacesenginewindows64only20140513.zip",
                    name: "DarkPlaces",
                    author: "LadyHavoc",
                    executable: "darkplaces-sdl.exe",
                    os: SourcePort.OperatingSystem.Win64
                ),
                new SourcePort // Linux32
                (
                    id: "darkplaces-linux32",
                    downloadUrl: "https://icculus.org/twilight/darkplaces/files/darkplacesengine20140513.zip",
                    name: "DarkPlaces",
                    author: "LadyHavoc",
                    executable: "darkplaces-linux-686-sdl",
                    os: SourcePort.OperatingSystem.Linux32
                ),
                new SourcePort // Linux64
                (
                    id: "darkplaces-linux64",
                    downloadUrl: "https://icculus.org/twilight/darkplaces/files/darkplacesengine20140513.zip",
                    name: "DarkPlaces",
                    author: "LadyHavoc",
                    executable: "darkplaces-linux-x86_64-sdl",
                    os: SourcePort.OperatingSystem.Linux64
                ),
                // NQuake
                new SourcePort // Win32
                (
                    id: "nquake-win32",
                    downloadUrl: null,
                    name: "NQuake",
                    author: "NQuake Team",
                    executable: "ezquake.exe",
                    os: SourcePort.OperatingSystem.Win32
                ),
                new SourcePort // Linux32
                (
                    id: "nquake-linux32",
                    downloadUrl: null,
                    name: "NQuake",
                    author: "NQuake Team",
                    executable: "ezquake-gl.glx",
                    os: SourcePort.OperatingSystem.Linux32
                ),
                new SourcePort // Linux64
                (
                    id: "nquake-linux64",
                    downloadUrl: null,
                    name: "NQuake",
                    author: "NQuake Team",
                    executable: "ezquake-gl.glx",
                    os: SourcePort.OperatingSystem.Linux64
                ),
            };
        }
        #endregion

        #region Methods
        public Task RefreshAsync()
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}
