using Avalonia.Media;
using LibQSelect;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.ViewModels
{
    public class SourcePortViewModel : INotifyPropertyChanged
    {
        #region Properties
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

        public bool IsDownloaded => SourcePort.IsDownloaded;
        public bool IsNotDownloaded => !IsDownloaded;

        public bool IsInactive => GameService.Game?.LoadedSourcePort != SourcePort;
        public bool CanMakeActive => IsInactive && IsDownloaded;
        public FontWeight NameFontWeight => !IsInactive ? FontWeight.Bold : FontWeight.Regular;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public SourcePortViewModel(SourcePort sourcePort)
        {
            SourcePort = sourcePort;

            sourcePort.PropertyChanged += ModelPropertyChanged;
            GameService.Initialised += GameService_Initialised;
        }
        #endregion

        #region Methods
        private void GameService_Initialised()
        {
            GameService.Game.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SourcePort.IsDownloaded):
                    PropertyChanged?.Invoke(this, new(nameof(IsDownloaded)));
                    PropertyChanged?.Invoke(this, new(nameof(IsNotDownloaded)));
                    PropertyChanged?.Invoke(this, new(nameof(CanMakeActive)));
                    break;
                case nameof(GameManager.LoadedSourcePort):
                    PropertyChanged?.Invoke(this, new(nameof(IsInactive)));
                    PropertyChanged?.Invoke(this, new(nameof(NameFontWeight)));
                    PropertyChanged?.Invoke(this, new(nameof(CanMakeActive)));
                    break;
            }
        }
        #endregion
    }
}
