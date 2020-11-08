using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LibQuakePackageManager.Providers;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace QSelectAvalonia.Views
{
    public class PackageArtView : UserControl
    {
        public Package Package { get; }

        public MemoryStream ImageStream { get; protected set; }

        protected Image PackageImage;
        protected TextBlock TitleTextBlock;
        protected TextBlock AuthorTextBlock;
        protected TextBlock RatingTextBlock;
        protected Panel DarkenPanel;
        protected TextBlock TickTextBlock;

        protected const int imageSideLength = 270;

        public PackageArtView()
        {
            this.InitializeComponent();
        }

        public PackageArtView(Package package) : this()
        {
            Package = package ?? throw new System.ArgumentNullException(nameof(package));

            if (package.Attributes.ContainsKey("Title")) TitleTextBlock.Text = package.Attributes["Title"];
            else TitleTextBlock.Text = package.Id;

            if (package.Attributes.ContainsKey("Author")) AuthorTextBlock.Text = package.Attributes["Author"];
            else AuthorTextBlock.IsVisible = false;

            if (package.Attributes.ContainsKey("Rating")) RatingTextBlock.Text = package.Attributes["Rating"];
            else RatingTextBlock.IsVisible = false;

            MarkAsDownloaded(package.IsDownloaded);

            LoadImageAsync().ConfigureAwait(false);
        }

        public void MarkAsDownloaded(bool downloaded)
        {
            DarkenPanel.IsVisible = TickTextBlock.IsVisible = downloaded;
        }

        public async Task LoadImageAsync()
        {
            if (PackageImage.Source == null)
            {
                // Set package image
                if (Package.Attributes.ContainsKey("Screenshot"))
                {
                    byte[] data;
                    using (WebClient client = new WebClient())
                    {
                        data = await client.DownloadDataTaskAsync(Package.Attributes["Screenshot"]);
                    }
                    ImageStream = new MemoryStream(data);
                    Bitmap bitmap = new Bitmap(ImageStream);
                    ImageStream.Seek(0, SeekOrigin.Begin);
                    CroppedBitmap croppedBitmap;
                    if (bitmap.PixelSize.Height > bitmap.PixelSize.Width)
                    {
                        bitmap = Bitmap.DecodeToWidth(ImageStream, imageSideLength);
                        int y = (bitmap.PixelSize.Height - bitmap.PixelSize.Width) / 2;
                        croppedBitmap = new CroppedBitmap(bitmap, new PixelRect(0, y, bitmap.PixelSize.Width, bitmap.PixelSize.Width));
                    }
                    else
                    {
                        bitmap = Bitmap.DecodeToHeight(ImageStream, imageSideLength);
                        int x = (bitmap.PixelSize.Width - bitmap.PixelSize.Height) / 2;
                        croppedBitmap = new CroppedBitmap(bitmap, new PixelRect(x, 0, bitmap.PixelSize.Height, bitmap.PixelSize.Height));
                    }
                    ImageStream.Seek(0, SeekOrigin.Begin);

                    PackageImage.Source = croppedBitmap;        
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            PackageImage = this.FindControl<Image>("PackageImage");
            TitleTextBlock = this.FindControl<TextBlock>("TitleTextBlock");
            AuthorTextBlock = this.FindControl<TextBlock>("AuthorTextBlock");
            RatingTextBlock = this.FindControl<TextBlock>("RatingTextBlock");
            DarkenPanel = this.FindControl<Panel>("DarkenPanel");
            TickTextBlock = this.FindControl<TextBlock>("TickTextBlock");

            this.PointerEnter += (a, b) => Background = Brushes.LightGray;
            this.PointerLeave += (a, b) => Background = Brushes.Transparent;
        }
    }
}
