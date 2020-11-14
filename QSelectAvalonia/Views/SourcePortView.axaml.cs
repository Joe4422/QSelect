using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QSelectAvalonia.Views
{
    public class SourcePortView : UserControl
    {
        public SourcePortView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
