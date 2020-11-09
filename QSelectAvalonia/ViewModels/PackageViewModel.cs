using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LibQuakePackageManager.Providers;
using QSelectAvalonia.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.ViewModels
{
    public class PackageViewModel : ReactiveObject
    {
        #region Properties
        public Package Package { get; }

        public string Title => Package.GetAttribute("Title") ?? Package.Id;
        public string Author => Package.GetAttribute("Author") ?? "";
        public string Rating => Package.GetAttribute("Rating") ?? "";

        protected IImage image = null;
        public IImage Image
        {
            get => image;
            set => this.RaiseAndSetIfChanged(ref image, value);
        }

        public bool HasAuthor => Author != "";
        public bool HasRating => Rating != "";

        public bool IsDownloaded => Package.IsDownloaded;
        #endregion Properties

        #region Constructors
        public PackageViewModel(Package package)
        {
            Package = package;
        }
        #endregion

        #region Methods
        public async Task LoadImageAsync(int imageSideLength)
        {
            if (Package.HasAttribute("Screenshot"))
            {
                byte[] data = await PackageImageService.GetBitmapAsync(Package);
                using MemoryStream ms = new MemoryStream(data);
                Bitmap bitmap = new Bitmap(ms);
                ms.Seek(0, SeekOrigin.Begin);
                CroppedBitmap croppedBitmap;
                if (bitmap.PixelSize.Height > bitmap.PixelSize.Width)
                {
                    bitmap = Bitmap.DecodeToWidth(ms, imageSideLength);
                    int y = (bitmap.PixelSize.Height - bitmap.PixelSize.Width) / 2;
                    croppedBitmap = new CroppedBitmap(bitmap, new PixelRect(0, y, bitmap.PixelSize.Width, bitmap.PixelSize.Width));
                }
                else
                {
                    bitmap = Bitmap.DecodeToHeight(ms, imageSideLength);
                    int x = (bitmap.PixelSize.Width - bitmap.PixelSize.Height) / 2;
                    croppedBitmap = new CroppedBitmap(bitmap, new PixelRect(x, 0, bitmap.PixelSize.Height, bitmap.PixelSize.Height));
                }

                Image = croppedBitmap;
            }
        }
        #endregion
    }
}
