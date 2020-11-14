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
        protected Button DownloadButton;

        public PackageWindowViewModel ViewModel { get; } = null;

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
            DownloadButton = this.FindControl<Button>("DownloadButton");

            PlayNowButton.Click += PlayNowButton_ClickAsync;
            DownloadButton.Click += DownloadButton_ClickAsync;
        }

        private async void DownloadButton_ClickAsync(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ViewModel.IsDownloaded) return;
            else
            {
                await DownloadService.DownloadItemAsync(ViewModel.Package);
            }
        }

        private async void PlayNowButton_ClickAsync(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!ViewModel.Package.IsDownloaded || GameService.Game?.LoadedSourcePort == null) return;
            else
            {
                await GameService.Game.LoadPackageAsync(ViewModel.Package.Id, ViewModel.Package);

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
