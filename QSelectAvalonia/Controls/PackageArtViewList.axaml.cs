using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LibQuakePackageManager.Providers;
using QSelectAvalonia.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QSelectAvalonia.Controls
{
    public class PackageArtViewList : UserControl
    {
        protected WrapPanel PackageWrapPanel;
        protected ScrollViewer PackageScrollViewer;
        protected Panel DarkenPanel;
        protected PackageWindow PackageWindow;

        protected List<Package> packages;

        protected bool isPackageWindowInUse = false;

        protected const int packagesPerLoad = 50;

        protected int numLoads = 0;

        public PackageArtViewList()
        {
            this.InitializeComponent();
        }

        public PackageArtViewList(List<Package> packages) : this()
        {
            this.packages = packages;

            LoadNextPackageSetAsync().ConfigureAwait(false);
            PackageScrollViewer.ScrollToHome();
        }

        protected async Task LoadNextPackageSetAsync()
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                foreach (Package package in packages.GetRange(numLoads * packagesPerLoad, packagesPerLoad))
                {
                    PackageArtView pav = new PackageArtView(package);
                    pav.Tapped += Pav_Tapped;
                    PackageWrapPanel.Children.Add(pav);
                }
                numLoads++;
            });
        }

        private void Pav_Tapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PackageWindow.DisplayPackage((sender as PackageArtView).ViewModel.Package);
            DarkenPanel.IsVisible = true;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            PackageWrapPanel = this.FindControl<WrapPanel>("PackageWrapPanel");
            PackageScrollViewer = this.FindControl<ScrollViewer>("PackageScrollViewer");
            DarkenPanel = this.FindControl<Panel>("DarkenPanel");
            PackageWindow = this.FindControl<PackageWindow>("PackageWindow");

            PackageScrollViewer.ScrollChanged += PackageScrollViewer_ScrollChanged;
            DarkenPanel.Tapped += DarkenPanel_Tapped;

            PackageWindow.PointerEnter += (a, b) => isPackageWindowInUse = true;
            PackageWindow.PointerLeave += (a, b) => isPackageWindowInUse = false;
        }

        private void DarkenPanel_Tapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!isPackageWindowInUse) DarkenPanel.IsVisible = false;
        }

        private void PackageScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;

            var delta = Math.Abs(ScrollViewer.VerticalScrollBarMaximumProperty.Getter(sv) - sv.Offset.Y);
            if (delta <= double.Epsilon)
            {
                LoadNextPackageSetAsync().ConfigureAwait(false);
            }
        }
    }
}
