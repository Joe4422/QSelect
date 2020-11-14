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
        protected ScrollViewer ScrollViewer;
        protected TextBlock TitleTextBlock;
        protected ListBox AttributesListBox;
        protected TabControl TabControl;
        protected FormattedTextBlock DescriptionTextBlock;
        protected TabItem DependenciesTabItem;
        protected ListBox DependenciesListBox;
        protected Image ImageImage;
        protected Button PlayNowButton;
        protected Button DownloadButton;

        public Package Package { get; protected set; }

        public delegate void RunPackageEventHandler(object sender, Package package);
        public event RunPackageEventHandler RunPackage;

        public delegate void DownloadPackageEventHandler(object sender, Package package);
        public event DownloadPackageEventHandler DownloadPackage;

        public PackageWindow()
        {
            this.InitializeComponent();
        }

        public void DisplayPackage(Package package)
        {
            Package = package;

            TabControl.SelectedIndex = 0;

            // Set up title
            if (package.Attributes.ContainsKey("Title")) TitleTextBlock.Text = package.Attributes["Title"];
            else TitleTextBlock.Text = package.Id;

            // Set up attributes
            List<string> atts = package.Attributes.Where(x => x.Key != "Title" && x.Key != "Description" && x.Key != "Screenshot").Select(x => $"{x.Key}: {x.Value}").ToList();
            atts.Add($"ID: {package.Id}");
            if (package.IsDownloaded) atts.Add($"Path: {package.InstallPath}");
            AttributesListBox.Items = atts;

            // Set up description
            if (package.Attributes.ContainsKey("Description")) DescriptionTextBlock.Text = package.Attributes["Description"];
            else DescriptionTextBlock.Text = "Unknown package.";

            // Set up dependencies
            if (package.Dependencies != null)
            {
                DependenciesTabItem.IsVisible = true;
                DependenciesListBox.Items = package.Dependencies;
            }
            else
            {
                DependenciesTabItem.IsVisible = false;
            }

            // Load image
            DisplayImageAsync().ConfigureAwait(false);

            if (package.IsDownloaded)
            {
                PlayNowButton.IsEnabled = true;
                DownloadButton.IsEnabled = false;
            }
            else
            {
                PlayNowButton.IsEnabled = false;
                DownloadButton.IsEnabled = true;
            }

        }

        protected async Task DisplayImageAsync()
        {
            // Set package image
            ImageImage.IsVisible = false;

            if (ImageImage.Source != null)
            {
                (ImageImage.Source as Bitmap).Dispose();
            }

            if (Package.Attributes.ContainsKey("ScreenshotURL"))
            {
                ImageImage.Source = await PackageImageService.GetScreenshotAsync(Package);
                if (ImageImage.Source != null) ImageImage.IsVisible = true;
            }
            ScrollViewer.ScrollToHome();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            ScrollViewer = this.FindControl<ScrollViewer>("ScrollViewer");
            TitleTextBlock = this.FindControl<TextBlock>("TitleTextBlock");
            AttributesListBox = this.FindControl<ListBox>("AttributesListBox");
            TabControl = this.FindControl<TabControl>("TabControl");
            DescriptionTextBlock = this.FindControl<FormattedTextBlock>("DescriptionTextBlock");
            DependenciesTabItem = this.FindControl<TabItem>("DependenciesTabItem");
            DependenciesListBox = this.FindControl<ListBox>("DependenciesListBox");
            ImageImage = this.FindControl<Image>("ImageImage");
            PlayNowButton = this.FindControl<Button>("PlayNowButton");
            DownloadButton = this.FindControl<Button>("DownloadButton");

            PlayNowButton.Click += PlayNowButton_ClickAsync;
            DownloadButton.Click += DownloadButton_ClickAsync;
        }

        private async void DownloadButton_ClickAsync(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Package.IsDownloaded) return;
            else
            {
                await DownloadService.DownloadItemAsync(Package);
                PlayNowButton.IsEnabled = true;
                DownloadButton.IsEnabled = false;
            }
        }

        private async void PlayNowButton_ClickAsync(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!Package.IsDownloaded) return;
            else
            {
                SourcePort port = DatabaseService.SourcePorts["quakespasm-spiked-win64"];
                if (port.IsDownloaded == false)
                {
                    await DownloadService.DownloadItemAsync(port);
                }
                GameService.Game.LoadSourcePort(port);

                await GameService.Game.LoadPackageAsync(Package.Id, Package);

                await GameService.Game.ExecuteLoadedSourcePortAsync();

                await GameService.Game.UnloadAllPackagesAsync();
            }
        }
    }
}
