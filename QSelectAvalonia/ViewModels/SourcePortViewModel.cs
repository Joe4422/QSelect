using Avalonia.Media;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.ViewModels
{
    public class SourcePortViewModel
    {
        public SourcePort SourcePort { get; }

        public string Name => SourcePort.Name;
        public string Author => SourcePort.Author;
        public string OS => SourcePort.SupportedOS switch
        {
            SourcePort.OperatingSystem.Win32 => "Windows (x86)",
            SourcePort.OperatingSystem.Win64 => "Windows (x64)",
            SourcePort.OperatingSystem.Linux32 => "Linux (x86)",
            SourcePort.OperatingSystem.Linux64 => "Linux (x86)",
            SourcePort.OperatingSystem.MacOS => "macOS",
            _ => "Unknown OS"
        };
        public FontWeight NameFontWeight => !IsInactive ? FontWeight.Bold : FontWeight.Regular;
        public bool IsDownloaded => SourcePort.IsDownloaded;
        public bool IsNotDownloaded => !SourcePort.IsDownloaded;
        public bool IsInactive => GameService.Game?.LoadedSourcePort != SourcePort;

        public SourcePortViewModel(SourcePort sourcePort)
        {
            SourcePort = sourcePort;
        }
    }
}
