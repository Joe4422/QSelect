using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QSelectAvalonia.Pages
{
    public class ModPage : UserControl
    {
        public ModPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
