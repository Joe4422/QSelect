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
        protected TextBlock SplashTextBlock;

        public MainWindow()
        {
            InitializeComponent();

            GetSplashTextAsync().ConfigureAwait(false);

            InitPackageListAsync().ConfigureAwait(false);

        }

        protected async Task GetSplashTextAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = "QSelectAvalonia.Assets.splashes.txt";

            List<string> splashes;

            using (Stream stream = assembly.GetManifestResourceStream(resource))
                using (StreamReader reader = new StreamReader(stream))
            {
                splashes = (await reader.ReadToEndAsync()).Split("\n").ToList();
            }

            Random random = new Random();
            SplashTextBlock.Text = splashes[random.Next(splashes.Count)];
        }

        protected async Task InitPackageListAsync()
        {
            await DatabaseService.InitialiseAsync("Packages", "SourcePorts");
            DownloadService.Initialise("Downloads", "Packages", "SourcePorts");
            GameService.Initialise(new Settings(), DatabaseService.Packages);

            //PackagesTabItem.Content = new PackageArtViewList(DatabaseService.Packages.Items);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            SourcePortsTabItem = this.FindControl<TabItem>("SourcePortsTabItem");
            PackagesTabItem = this.FindControl<TabItem>("PackagesTabItem");
            DownloadsTabItem = this.FindControl<TabItem>("DownloadsTabItem");
            SettingsTabItem = this.FindControl<TabItem>("SettingsTabItem");
            SplashTextBlock = this.FindControl<TextBlock>("SplashTextBlock");
        }
    }
}
