using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QSelectAvalonia.Controls;
using QSelectAvalonia.Services;
using System.Threading.Tasks;

namespace QSelectAvalonia.Pages
{
    public class PackagesPage : UserControl
    {
        protected Panel FilterPanel;
        protected Panel PackagesPanel;

        public PackagesPage()
        {
            this.InitializeComponent();

            DatabaseService.Initialised += DatabaseService_Initialised;
        }

        private void DatabaseService_Initialised()
        {
            PackagesPanel.Children.Add(new PackageArtViewList());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            FilterPanel = this.FindControl<Panel>("FilterPanel");
            PackagesPanel = this.FindControl<Panel>("PackagesPanel");
        }
    }
}
