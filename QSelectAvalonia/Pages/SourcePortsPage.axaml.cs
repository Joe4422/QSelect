using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.Services;
using QSelectAvalonia.Views;
using System.Collections.Generic;
using System.Linq;
using LibPackageManager.Repositories;

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
            Dictionary<string, List<SourcePort>> groups = new();
            foreach (SourcePort sp in DatabaseService.SourcePorts.Items.Where(x => x.DownloadUrl != null))
            {
                string id = string.Join('-', sp.Id.Split('-', StringSplitOptions.RemoveEmptyEntries)[..^1]);
                if (!groups.ContainsKey(id)) groups[id] = new();
                groups[id].Add(sp);

            }
            foreach (var group in groups)
            {
                SourcePortView spv = new(group.Value);
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
            if (sourcePort.Token.State is not ProgressToken.ProgressState.NotStarted or ProgressToken.ProgressState.Failed) return;
            else
            {
                await DownloadService.SourcePorts.GetItemAsync(sourcePort);
            }
        }
    }
}
