using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibQSelect.PackageManager.Packages;
using QSelectAvalonia.Controls;
using QSelectAvalonia.Services;
using QSelectAvalonia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QSelectAvalonia.Pages
{
    public class PackagesPage : UserControl
    {
        protected Panel FilterPanel;
        protected Panel PackagesPanel;
        protected PackageWindow PackageWindow;
        protected PackageArtViewList PackageArtViewList;
        protected TextBox SearchTextBox;

        protected List<(string text, Func<Package, bool> filter)> filters = new()
        {
            new("All Packages", (x) => true),
            new("Downloaded", (x) => x.IsDownloaded),
            new("★★★★★", (x) => x.GetAttribute("Rating") == "★★★★★")
        };

        //$"{new string('★', rint)}{new string('☆', 5 - rint)}";

        public PackagesPage()
        {
            this.InitializeComponent();

            DatabaseService.Initialised += DatabaseService_Initialised;

            foreach (var filter in filters)
            {
                Button btn = new Button()
                {
                    Content = filter.text
                };
                btn.Click += FilterButton_Click;
                FilterPanel.Children.Add(btn);
            }
        }

        private void FilterButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                PackageArtViewList.SetFilter(filters.First(x => btn.Content as string == x.text).filter);
            }
        }

        private void DatabaseService_Initialised()
        {
            PackageArtViewList = new();
            PackagesPanel.Children.Add(PackageArtViewList);
            PackageArtViewList.PackageSelected += PackageArtViewList_PackageSelected;
        }

        private void PackageArtViewList_PackageSelected(Package package)
        {
            PackageWindow.DisplayPackage(package);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            FilterPanel = this.FindControl<Panel>("FilterPanel");
            PackagesPanel = this.FindControl<Panel>("PackagesPanel");
            PackageWindow = this.FindControl<PackageWindow>("PackageWindow");

            SearchTextBox = new TextBox() { Watermark = "Search..." };
            SearchTextBox.KeyDown += SearchTextBox_KeyDown;
            FilterPanel.Children.Add(SearchTextBox);
        }

        private void SearchTextBox_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                string text = SearchTextBox.Text.ToLower();
                Func<Package, bool> filter = (x) =>
                {
                    bool valid = false;
                    valid |= x.Id.ToLower().Contains(text);
                    if (x.HasAttribute("Title")) valid |= x.Attributes["Title"].ToLower().Contains(text);
                    if (x.HasAttribute("Author")) valid |= x.Attributes["Author"].ToLower().Contains(text);

                    return valid;
                };
                PackageArtViewList.SetFilter(filter);
            }
        }
    }
}
