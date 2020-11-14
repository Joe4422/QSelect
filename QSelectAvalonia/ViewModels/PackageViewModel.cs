using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LibQSelect.PackageManager.Packages;
using QSelectAvalonia.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QSelectAvalonia.ViewModels
{
    public class PackageViewModel : INotifyPropertyChanged
    {
        #region Properties
        public Package Package { get; }

        public string Title => Package.GetAttribute("Title") ?? Package.Id;
        public string Author => Package.GetAttribute("Author") ?? "";
        public bool HasAuthor => Author != "";
        public string Rating => Package.GetAttribute("Rating") ?? "";
        public bool HasRating => Rating != "";

        protected IImage thumbnail = null;
        public IImage Thumbnail
        {
            get => thumbnail;
            set
            {
                thumbnail = value;
                PropertyChanged?.Invoke(this, new(nameof(Thumbnail)));
            }
        }

        public bool IsDownloaded => Package.IsDownloaded;
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public PackageViewModel(Package package)
        {
            Package = package;

            Package.PropertyChanged += Package_PropertyChanged;
        }
        #endregion

        #region Methods
        public async Task LoadThumbnailAsync()
        {
            if (Package.HasAttribute("ThumbnailURL"))
            {
                Thumbnail = await PackageImageService.GetThumbnailAsync(Package);
            }
        }

        private void Package_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Package.IsDownloaded):
                    PropertyChanged?.Invoke(this, new(nameof(IsDownloaded)));
                    break;
            }
        }
        #endregion
    }
}
