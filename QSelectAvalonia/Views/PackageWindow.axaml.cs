using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using LibQuakePackageManager.Providers;
using QSelectAvalonia.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

        protected Package package;

        public PackageWindow()
        {
            this.InitializeComponent();
        }

        public void DisplayPackage(Package package, MemoryStream imageStream)
        {
            this.package = package;

            TabControl.SelectedIndex = 0;

            // Set up title
            if (package.Attributes.ContainsKey("Title")) TitleTextBlock.Text = package.Attributes["Title"];
            else TitleTextBlock.Text = package.Id;

            // Set up attributes
            List<string> atts = package.Attributes.Where(x => x.Key != "Title" && x.Key != "Description" && x.Key != "Screenshot").Select(x => $"{x.Key}: {x.Value}").ToList();
            atts.Add($"ID: {package.Id}");
            if (package.IsDownloaded) atts.Add($"Path: {package.InstallDirectory}");
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
            DisplayImage(imageStream);
        }

        protected void DisplayImage(MemoryStream imageStream)
        {
            // Set package image
            if (imageStream != null && package.Attributes.ContainsKey("Screenshot"))
            {
                ImageImage.IsVisible = false;
                imageStream.Seek(0, SeekOrigin.Begin);
                try
                {
                    Bitmap bitmap = new Bitmap(imageStream);

                    ImageImage.Source = bitmap;
                    ImageImage.IsVisible = true;
                    ScrollViewer.ScrollToHome();
                }
                catch (Exception) { }
            }
            else
            {
                ImageImage.IsVisible = false;
            }
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
        }
    }
}
