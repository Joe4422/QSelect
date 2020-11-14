using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibQSelect.PackageManager.SourcePorts;
using QSelectAvalonia.ViewModels;

namespace QSelectAvalonia.Views
{
    public class SourcePortView : UserControl
    {
        public SourcePortViewModel ViewModel { get; }

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
        }
    }
}
