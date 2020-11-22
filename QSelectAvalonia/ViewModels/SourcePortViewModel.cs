using Avalonia.Media;
using LibPackageManager.Repositories;
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

        public bool IsInstalled => SourcePort.Token.State is ProgressToken.ProgressState.Installed;
        public bool IsNotInstalled => !IsInstalled;

        public bool IsInactive => GameService.Game?.LoadedSourcePort != SourcePort;
        public bool CanMakeActive => IsInactive && IsInstalled;
        public FontWeight NameFontWeight => !IsInactive ? FontWeight.Bold : FontWeight.Regular;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public SourcePortViewModel(SourcePort sourcePort)
        {
            SourcePort = sourcePort;

            sourcePort.Token.PropertyChanged += ModelPropertyChanged;
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
                case nameof(ProgressToken.State):
                    PropertyChanged?.Invoke(this, new(nameof(IsInstalled)));
                    PropertyChanged?.Invoke(this, new(nameof(IsNotInstalled)));
                    PropertyChanged?.Invoke(this, new(nameof(CanMakeActive)));
                    break;
                case nameof(GameLauncher.LoadedSourcePort):
                    PropertyChanged?.Invoke(this, new(nameof(IsInactive)));
                    PropertyChanged?.Invoke(this, new(nameof(NameFontWeight)));
                    PropertyChanged?.Invoke(this, new(nameof(CanMakeActive)));
                    break;
            }
        }
        #endregion
    }
}
