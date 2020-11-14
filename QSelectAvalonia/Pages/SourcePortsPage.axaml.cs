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
                SourcePortStackPanel.Children.Add(new SourcePortView(sp));
            }
        }
    }
}
