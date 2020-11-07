using Config.Net;
using System.Collections.Generic;

namespace LibQSelect
{
    public interface IAppSettings
    {
        [Option(Alias = "Paths.SourcePortsDirectory", DefaultValue = "SourcePorts")]
        string SourcePortsDirectory { get; set; }

        [Option(Alias = "Paths.ModsDirectory", DefaultValue = "Mods")]
        string ModsDirectory { get; set; }

        [Option(Alias = "Paths.SyncQuakeConfig", DefaultValue = true)]
        bool SyncQuakeConfig { get; set; }

        [Option(Alias = "State.LastSourcePort", DefaultValue = "")]
        string LastSourcePort { get; set; }

        [Option(Alias = "State.EnabledMods", DefaultValue = "")]
        string EnabledMods { get; set; }
    }
}
