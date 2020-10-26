using Config.Net;
using System.Collections.Generic;

namespace QSelect
{
    public interface IAppSettings
    {
        [Option(Alias = "Paths.LastSelected", DefaultValue = "")]
        string LastSelected { get; set; }

        [Option(Alias = "Paths.Executable", DefaultValue = "quakespasm.exe")]
        string Executable { get; set; }

        [Option(Alias = "Paths.GameDirectory", DefaultValue = "Game")]
        string GameDirectory { get; set; }

        [Option(Alias = "Paths.ContentDirectory", DefaultValue = "Content")]
        string ContentDirectory { get; set; }

        [Option(Alias = "Paths.SyncQuakeConfig", DefaultValue = true)]
        bool SyncQuakeConfig { get; set; }

        [Option(Alias = "Paths.QuakeConfigPath", DefaultValue = "Content/QuakeConfig.cfg")]
        string QuakeConfigPath { get; set; }
    }
}
