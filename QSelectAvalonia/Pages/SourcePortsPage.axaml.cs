using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.Services;
using QSelectAvalonia.Views;
using System.Linq;

namespace QSelectAvalonia.Pages
{
    public class SourcePortsPage : UserControl
    {
        protected StackPanel SourcePortStackPanel;

        public SourcePortsPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            SourcePortStackPanel = this.FindControl<StackPanel>("SourcePortStackPanel");

            DatabaseService.Initialised += DatabaseService_Initialised;
        }

        private void DatabaseService_Initialised()
        {
            foreach (SourcePort sp in DatabaseService.SourcePorts.Items.Where(x => x.DownloadUrl != null))
            {
                SourcePortView spv = new(sp);
                spv.Download += Spv_Download;
                spv.MakeActive += Spv_MakeActive;

                SourcePortStackPanel.Children.Add(spv);
            }
        }

        private void Spv_MakeActive(object sender, SourcePort sourcePort)
        {
            GameService.Game?.LoadSourcePort(sourcePort);
        }

        private async void Spv_Download(object sender, SourcePort sourcePort)
        {
            if (sourcePort.IsDownloaded) return;
            else
            {
                await DownloadService.DownloadItemAsync(sourcePort);
            }
        }
    }
}
