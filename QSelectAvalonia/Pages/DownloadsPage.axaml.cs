using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QSelectAvalonia.Pages
{
    public class DownloadsPage : UserControl
    {
        public DownloadsPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
