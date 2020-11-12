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
                    "quakespasm-win32",
                    "Quakespasm",
                    "Fitzgibbons",
                    "quakespasm.exe",
                    null,
                    SourcePort.OperatingSystem.Win32
                ),
                new SourcePort // Win64
                (
                    "quakespasm-win64",
                    "Quakespasm",
                    "Fitzgibbons",
                    "quakespasm.exe",
                    null,
                    SourcePort.OperatingSystem.Win64
                ),
                new SourcePort // Linux32
                (
                    "quakespasm-linux32",
                    "Quakespasm",
                    "Fitzgibbons",
                    "quakespasm",
                    null,
                    SourcePort.OperatingSystem.Linux32
                ),
                new SourcePort // Linux64
                (
                    "quakespasm-linux64",
                    "Quakespasm",
                    "Fitzgibbons",
                    "quakespasm",
                    null,
                    SourcePort.OperatingSystem.Linux64
                ),
                // Quakespasm Spiked
                new SourcePort // Win32
                (
                    "quakespasm-spiked-win32",
                    "Quakespasm Spiked",
                    "Shpoike",
                    "quakespasm-spiked-win32.exe",
                    "https://fte.triptohell.info/moodles/qss/quakespasm_spiked_win32.zip",
                    SourcePort.OperatingSystem.Win32
                ),
                new SourcePort // Win64
                (
                    "quakespasm-spiked-win64",
                    "Quakespasm Spiked",
                    "Shpoike",
                    "quakespasm-spiked-win64.exe",
                    "https://fte.triptohell.info/moodles/qss/quakespasm_spiked_win64.zip",
                    SourcePort.OperatingSystem.Win64
                ),
                new SourcePort // Linux64
                (
                    "quakespasm-spiked-linux64",
                    "Quakespasm Spiked",
                    "Shpoike",
                    "quakespasm-spiked-linux64",
                    "https://fte.triptohell.info/moodles/qss/quakespasm_spiked_linux64.zip",
                    SourcePort.OperatingSystem.Linux64
                ),
                // DarkPlaces
                new SourcePort // Win32
                (
                    "darkplaces-win32",
                    "DarkPlaces",
                    "LadyHavoc",
                    "darkplaces-sdl.exe",
                    "https://icculus.org/twilight/darkplaces/files/darkplacesenginewindowsonly20140513.zip",
                    SourcePort.OperatingSystem.Win32
                ),
                new SourcePort // Win64
                (
                    "darkplaces-win64",
                    "DarkPlaces",
                    "LadyHavoc",
                    "darkplaces-sdl.exe",
                    "https://icculus.org/twilight/darkplaces/files/darkplacesenginewindows64only20140513.zip",
                    SourcePort.OperatingSystem.Win64
                ),
                new SourcePort // Linux32
                (
                    "darkplaces-linux32",
                    "DarkPlaces",
                    "LadyHavoc",
                    "darkplaces-linux-686-sdl",
                    "https://icculus.org/twilight/darkplaces/files/darkplacesengine20140513.zip",
                    SourcePort.OperatingSystem.Linux32
                ),
                new SourcePort // Linux64
                (
                    "darkplaces-linux64",
                    "DarkPlaces",
                    "LadyHavoc",
                    "darkplaces-linux-x86_64-sdl",
                    "https://icculus.org/twilight/darkplaces/files/darkplacesengine20140513.zip",
                    SourcePort.OperatingSystem.Linux64
                ),
                // NQuake
                new SourcePort // Win32
                (
                    "nquake-win32",
                    "NQuake",
                    "NQuake Team",
                    "ezquake.exe",
                    null,
                    SourcePort.OperatingSystem.Win32
                ),
                new SourcePort // Linux32
                (
                    "nquake-linux32",
                    "NQuake",
                    "NQuake Team",
                    "ezquake-gl.glx",
                    null,
                    SourcePort.OperatingSystem.Linux32
                ),
                new SourcePort // Linux64
                (
                    "nquake-linux64",
                    "NQuake",
                    "NQuake Team",
                    "ezquake-gl.glx",
                    null,
                    SourcePort.OperatingSystem.Linux64
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
