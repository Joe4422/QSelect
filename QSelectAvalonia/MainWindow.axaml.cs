using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibQSelect;
using LibQuakePackageManager.Databases;
using LibQuakePackageManager.Providers;
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
            List<IProvider<Package>> packageProviders = new List<IProvider<Package>>()
            {
                new LocalPackageProvider("Packages"),
                new BuiltInPackageProvider(),
                new QuaddictedPackageProvider()
            };
            List<IProvider<SourcePort>> sourcePortProviders = new List<IProvider<SourcePort>>()
            {
                new LocalSourcePortProvider("SourcePorts"),
                new BuiltInSourcePortProvider()
            };
            pdm = new PackageDatabaseManager("packages.json", packageProviders);
            spdm = new SourcePortDatabaseManager("sourceports.json", sourcePortProviders);
            
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
