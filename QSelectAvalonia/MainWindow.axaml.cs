using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibPackageManager.Repositories;
using LibQSelect;
using LibQSelect.PackageManager;
using QSelectAvalonia.Controls;
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

        protected PackageDatabaseManager pdm;
        protected SourcePortDatabaseManager spdm;

        public MainWindow()
        {
            InitializeComponent();

            InitPackageListAsync().ConfigureAwait(false);

        }

        protected async Task InitPackageListAsync()
        {
            List<IRepository<Package>> packageRepositories = new()
            {
                new InstalledPackageRepository("Packages"),
                new BuiltInPackageRepository(),
                new QuaddictedPackageRepository()
            };
            List<IRepository<SourcePort>> sourcePortRepositories = new()
            {
                new InstalledSourcePortRepository("SourcePorts"),
                new BuiltInSourcePortRepository()
            };
            pdm = new PackageDatabaseManager("packages.json", packageRepositories);
            spdm = new SourcePortDatabaseManager("sourceports.json", sourcePortRepositories);
            
            Task pdmTask = pdm.LoadDatabaseAsync();
            Task spdmTask = spdm.LoadDatabaseAsync();

            await Task.WhenAll(pdmTask, spdmTask);

            PackagesTabItem.Content = new PackageArtViewList(pdm.Items);
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
