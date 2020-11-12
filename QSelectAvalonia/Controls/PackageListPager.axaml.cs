using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibQSelect.PackageManager.Packages;
using QSelectAvalonia.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QSelectAvalonia.Controls
{
    public class PackageListPager : UserControl
    {
        protected StackPanel PackageStackPanel;
        protected Button LastPageButton;
        protected TextBlock PageLabel;
        protected Button NextPageButton;

        protected List<Package> packages = new List<Package>();
        protected List<PackageView> packageViews = new List<PackageView>();

        public int ItemsPerPage { get; } = 100;
        protected int currentPage = 0;
        public int CurrentPage
        {
            get => currentPage + 1;
            set
            {
                if (value < MinPage || value > MaxPage) return;
                currentPage = value - 1;
                UpdatePage();
            }
        }
        public int MinPage => 1;
        public int MaxPage => (packages.Count / ItemsPerPage) + 1;

        public PackageListPager()
        {
            this.InitializeComponent();
        }

        public PackageListPager(List<Package> packages) : this()
        {
            this.packages = packages ?? throw new ArgumentNullException(nameof(packages));

            packageViews = packages.Select(x => new PackageView(x)).ToList();

            UpdatePage();
        }

        protected void UpdatePage()
        {
            PackageStackPanel.Children.Clear();

            int startIndex = ItemsPerPage * currentPage;
            List<PackageView> pkgViewsInPage;
            if (startIndex + ItemsPerPage >= packages.Count)
            {
                pkgViewsInPage = packageViews.GetRange(startIndex, packages.Count - startIndex);
            }
            else
            {
                pkgViewsInPage = packageViews.GetRange(startIndex, ItemsPerPage);
            }

            foreach (PackageView pkgView in pkgViewsInPage)
            {
                PackageStackPanel.Children.Add(pkgView);
            }

            if (CurrentPage == MinPage) LastPageButton.IsEnabled = false;
            else LastPageButton.IsEnabled = true;

            if (CurrentPage == MaxPage) NextPageButton.IsEnabled = false;
            else NextPageButton.IsEnabled = true;

            PageLabel.Text = $"{CurrentPage}/{MaxPage}";

            GC.Collect();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            PackageStackPanel = this.FindControl<StackPanel>("PackageStackPanel");
            LastPageButton = this.FindControl<Button>("LastPageButton");
            PageLabel = this.FindControl<TextBlock>("PageLabel");
            NextPageButton = this.FindControl<Button>("NextPageButton");

            LastPageButton.Click += (a, b) => CurrentPage--;
            NextPageButton.Click += (a, b) => CurrentPage++;
        }
    }
}
