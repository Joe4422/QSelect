using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LibQSelect.PackageManager;
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
                Image = await PackageImageService.GetThumbnailAsync(Package, imageSideLength);
            }
        }
        #endregion
    }
}
