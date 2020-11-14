using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.Services;
using QSelectAvalonia.ViewModels;

namespace QSelectAvalonia.Views
{
    public class SourcePortView : UserControl
    {
        public SourcePortViewModel ViewModel { get; }

        protected Button DownloadButton;
        protected Button MakeActiveButton;

        public delegate void MakeActiveEventHandler(object sender, SourcePort sourcePort);
        public event MakeActiveEventHandler MakeActive;

        public delegate void DownloadEventHandler(object sender, SourcePort sourcePort);
        public event DownloadEventHandler Download;

        public SourcePortView()
        {
            this.InitializeComponent();
        }

        public SourcePortView(SourcePort sourcePort) : this()
        {
            ViewModel = new(sourcePort);

            DataContext = ViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            DownloadButton = this.FindControl<Button>("DownloadButton");
            MakeActiveButton = this.FindControl<Button>("MakeActiveButton");

            DownloadButton.Click += DownloadButton_Click;
            MakeActiveButton.Click += MakeActiveButton_Click;
        }

        private void MakeActiveButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MakeActive?.Invoke(this, ViewModel.SourcePort);
        }

        private void DownloadButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Download?.Invoke(this, ViewModel.SourcePort);

        }
    }
}
