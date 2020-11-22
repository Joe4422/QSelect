using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibPackageManager.Repositories;
using LibQSelect;
using LibQSelect.PackageManager.Packages;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.Controls;
using QSelectAvalonia.Services;
using QSelectAvalonia.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QSelectAvalonia
{
    public class MainWindow : Window
    {
        protected TabItem SourcePortsTabItem;
        protected TabItem PackagesTabItem;
        protected TabItem DownloadsTabItem;
        protected TabItem SettingsTabItem;

        public MainWindow()
        {
            InitializeComponent();

            InitPackageListAsync().ConfigureAwait(false);

        }

        protected async Task InitPackageListAsync()
        {
            await SettingsService.InitialiseAsync();
            await DatabaseService.InitialiseAsync(SettingsService.Settings.PackagesPath, SettingsService.Settings.SourcePortsPath);
            DownloadService.Initialise(SettingsService.Settings.DownloadsPath, SettingsService.Settings.PackagesPath, SettingsService.Settings.SourcePortsPath);
            GameService.Initialise();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            SourcePortsTabItem = this.FindControl<TabItem>("SourcePortsTabItem");
            PackagesTabItem = this.FindControl<TabItem>("PackagesTabItem");
            DownloadsTabItem = this.FindControl<TabItem>("DownloadsTabItem");
            SettingsTabItem = this.FindControl<TabItem>("SettingsTabItem");
        }
    }
}
