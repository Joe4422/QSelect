using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QSelectAvalonia.Pages
{
    public class EnginePage : UserControl
    {
        public EnginePage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
