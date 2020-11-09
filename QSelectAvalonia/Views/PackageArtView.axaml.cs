using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LibQuakePackageManager.Providers;
using QSelectAvalonia.ViewModels;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace QSelectAvalonia.Views
{
    public class PackageArtView : UserControl
    {
        public PackageViewModel ViewModel { get; }

        protected const int imageSideLength = 270;

        public PackageArtView()
        {
            this.InitializeComponent();
        }

        public PackageArtView(Package package) : this()
        {
            ViewModel = new PackageViewModel(package);

            ViewModel.LoadImageAsync(imageSideLength).ConfigureAwait(false);

            DataContext = ViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.PointerEnter += (a, b) => Background = Brushes.LightGray;
            this.PointerLeave += (a, b) => Background = Brushes.Transparent;
        }
    }
}
