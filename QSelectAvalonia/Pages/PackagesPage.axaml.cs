using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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
        protected Panel PackageWindowPanel;
        protected PackageWindow PackageWindow;
        protected PackageArtViewList PackageArtViewList;
        protected TextBox SearchTextBox;

        protected List<(string text, Func<Package, bool> filter)> filters = new()
        {
            new("All Packages", (x) => true),
            new("Downloaded", (x) => x.Token.State == LibPackageManager.Repositories.ProgressToken.ProgressState.Installed),
            new("★★★★★", (x) => x.GetAttribute("Rating") == "5")
        };

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
            PackageWindow = new PackageWindow(package);

            PackageWindowPanel.Children.Clear();
            PackageWindowPanel.Children.Add(PackageWindow);
            PackageWindowPanel.IsVisible = true;
            PackageWindow.GoBack += PackageWindow_GoBack;
        }

        private void PackageWindow_GoBack(object sender)
        {
            PackageWindowPanel.IsVisible = false;
            PackageWindowPanel.Children.Clear();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            FilterPanel = this.FindControl<Panel>("FilterPanel");
            PackagesPanel = this.FindControl<Panel>("PackagesPanel");
            PackageWindowPanel = this.FindControl<Panel>("PackageWindowPanel");

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
