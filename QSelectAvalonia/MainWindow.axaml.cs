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
using System.Collections.Generic;
using System.Linq;
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
            await DatabaseService.InitialiseAsync("Packages", "SourcePorts", "Thumbnails");
            DownloadService.Initialise("Downloads", "Packages", "SourcePorts", "Thumbnails");
            GameService.Initialise(new Settings(), DatabaseService.Packages);

            PackagesTabItem.Content = new PackageArtViewList(DatabaseService.Packages.Items);
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
