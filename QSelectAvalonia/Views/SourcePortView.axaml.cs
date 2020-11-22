using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.Services;
using QSelectAvalonia.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace QSelectAvalonia.Views
{
    public class SourcePortView : UserControl
    {
        public List<SourcePortViewModel> ViewModels { get; }

        protected Button DownloadButton;
        protected Button MakeActiveButton;
        protected ComboBox OSComboBox;

        public delegate void MakeActiveEventHandler(object sender, SourcePort sourcePort);
        public event MakeActiveEventHandler MakeActive;

        public delegate void DownloadEventHandler(object sender, SourcePort sourcePort);
        public event DownloadEventHandler Download;

        public SourcePortView()
        {
            this.InitializeComponent();
        }

        public SourcePortView(List<SourcePort> sourcePorts) : this()
        {
            ViewModels = sourcePorts.Select(x => new SourcePortViewModel(x)).ToList();

            OSComboBox.Items = ViewModels.Select(x => x.OS);

            List<SourcePort> installed = sourcePorts.Where(x => x.Token.State == LibPackageManager.Repositories.ProgressToken.ProgressState.Installed).ToList();
            if (installed.Count > 0) DataContext = ViewModels.First(x => x.SourcePort == installed[0]);
            else DataContext = ViewModels[0];

            OSComboBox.SelectedIndex = ViewModels.IndexOf(DataContext as SourcePortViewModel);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            DownloadButton = this.FindControl<Button>("DownloadButton");
            MakeActiveButton = this.FindControl<Button>("MakeActiveButton");
            OSComboBox = this.FindControl<ComboBox>("OSComboBox");

            DownloadButton.Click += DownloadButton_Click;
            MakeActiveButton.Click += MakeActiveButton_Click;
            OSComboBox.SelectionChanged += OSComboBox_SelectionChanged;
        }

        private void OSComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = ViewModels.First(x => x.OS == (string)OSComboBox.SelectedItem);
        }

        private void MakeActiveButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MakeActive?.Invoke(this, (DataContext as SourcePortViewModel).SourcePort);
        }

        private void DownloadButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Download?.Invoke(this, (DataContext as SourcePortViewModel).SourcePort);

        }
    }
}
