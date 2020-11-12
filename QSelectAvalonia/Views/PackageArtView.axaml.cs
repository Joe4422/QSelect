using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LibQSelect.PackageManager.Packages;
using QSelectAvalonia.ViewModels;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace QSelectAvalonia.Views
{
    public class PackageArtView : UserControl
    {
        public PackageViewModel ViewModel { get; }

        protected const int imageSideLength = 120;

        protected Image PackageImage;

        public PackageArtView()
        {
            this.InitializeComponent();
        }

        public PackageArtView(Package package) : this()
        {
            ViewModel = new PackageViewModel(package);

            ViewModel.LoadThumbnailAsync(imageSideLength).ConfigureAwait(false);

            DataContext = ViewModel;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Imaage")
            {
                if (ViewModel.Thumbnail.Size.Width > ViewModel.Thumbnail.Size.Height)
                {
                    int x = ((int)ViewModel.Thumbnail.Size.Width - imageSideLength) / 2;
                    PackageImage.Clip = new RectangleGeometry(new Rect(x, 0, imageSideLength, imageSideLength));
                }
                else
                {
                    int y = ((int)ViewModel.Thumbnail.Size.Height - imageSideLength) / 2;
                    PackageImage.Clip = new RectangleGeometry(new Rect(0, y, imageSideLength, imageSideLength));
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            PackageImage = this.FindControl<Image>("PackageImage");

            this.PointerEnter += (a, b) => Background = Brushes.LightGray;
            this.PointerLeave += (a, b) => Background = Brushes.Transparent;
        }
    }
}
