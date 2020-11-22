using ABI.Windows.Devices.Bluetooth.Background;
using ABI.Windows.UI.WebUI;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using LibQSelect.PackageManager.Packages;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.Controls;
using QSelectAvalonia.Services;
using QSelectAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace QSelectAvalonia.Views
{
    public class PackageWindow : UserControl
    {
        protected Button PlayNowButton;
        protected Button InstallButton;
        protected Button BackButton;

        public PackageWindowViewModel ViewModel { get; } = null;

        public delegate void GoBackEventHandler(object sender);
        public event GoBackEventHandler GoBack;

        public PackageWindow()
        {
            this.InitializeComponent();
        }

        public PackageWindow(Package package) : this()
        {
            ViewModel = new PackageWindowViewModel(package);

            DataContext = ViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            PlayNowButton = this.FindControl<Button>("PlayNowButton");
            InstallButton = this.FindControl<Button>("InstallButton");
            BackButton = this.FindControl<Button>("BackButton");

            PlayNowButton.Click += PlayNowButton_ClickAsync;
            InstallButton.Click += InstallButton_ClickAsync;
            BackButton.Click += (a, b) => GoBack?.Invoke(this);
        }

        private async void InstallButton_ClickAsync(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel.IsInstalled) return;
            else
            {
                await DownloadService.Packages.GetItemAsync(ViewModel.Package);
            }
        }

        private async void PlayNowButton_ClickAsync(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!ViewModel.IsInstalled || GameService.Game?.LoadedSourcePort == null) return;
            else
            {
                await GameService.Game.LoadPackageAsync(ViewModel.Package);

                string args;
                if (ViewModel.Package.HasAttribute("StartMaps") && ViewModel.Package.GetAttribute("StartMaps") != "")
                {
                    args = $"+map {ViewModel.Package.GetAttribute("StartMaps").Split(" ", StringSplitOptions.RemoveEmptyEntries).First()}";
                }
                else args = "+map start";

                await GameService.Game.ExecuteLoadedSourcePortAsync(args);

                await GameService.Game.UnloadAllPackagesAsync();
            }
        }
    }
}
