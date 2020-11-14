using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LibQSelect.PackageManager.Packages;
using QSelectAvalonia.Services;
using QSelectAvalonia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QSelectAvalonia.Controls
{
    public class PackageArtViewList : UserControl
    {
        #region Controls
        protected WrapPanel PackageWrapPanel;
        protected ScrollViewer PackageScrollViewer;
        #endregion

        #region Variables
        protected List<Package> filteredPackages;
        protected const int packagesPerLoad = 50;
        protected int numLoads = 0;
        protected Func<Package, bool> filter = x => true;
        #endregion

        #region Events
        public delegate void PackageSelectedEventHandler(Package package);
        public event PackageSelectedEventHandler PackageSelected;
        #endregion

        #region Constructors
        public PackageArtViewList()
        {
            this.InitializeComponent();

            filteredPackages = DatabaseService.Packages.Items;

            LoadNextPackageSetAsync().ConfigureAwait(false);
            PackageScrollViewer.ScrollToHome();

        }
        #endregion

        #region Methods
        public void SetFilter(Func<Package, bool> filter)
        {
            this.filter = filter;

            filteredPackages = DatabaseService.Packages.Items.Where(x => filter(x)).ToList();

            PackageWrapPanel.Children.Clear();
            numLoads = 0;
            LoadNextPackageSetAsync().ConfigureAwait(false);
        }

        protected async Task LoadNextPackageSetAsync()
        {
            List<Package> pkgsToLoad;
            if (numLoads * packagesPerLoad > filteredPackages.Count) return;
            else if (numLoads * packagesPerLoad + packagesPerLoad > filteredPackages.Count) pkgsToLoad = filteredPackages.GetRange(numLoads * packagesPerLoad, filteredPackages.Count - (numLoads * packagesPerLoad));
            else pkgsToLoad = filteredPackages.GetRange(numLoads * packagesPerLoad, packagesPerLoad);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                foreach (Package pkg in pkgsToLoad)
                {
                    PackageArtView pav = new(pkg);
                    pav.Tapped += Pav_Tapped;
                    PackageWrapPanel.Children.Add(pav);
                }
            });

            numLoads++;
        }

        private void Pav_Tapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PackageSelected?.Invoke((sender as PackageArtView).ViewModel.Package);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            PackageWrapPanel = this.FindControl<WrapPanel>("PackageWrapPanel");
            PackageScrollViewer = this.FindControl<ScrollViewer>("PackageScrollViewer");

            PackageScrollViewer.ScrollChanged += PackageScrollViewer_ScrollChanged;
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
        #endregion
    }
}
