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
using LibPackageManager.Repositories;

namespace QSelectAvalonia.ViewModels
{
    public class PackageViewModel : INotifyPropertyChanged
    {
        #region Properties
        public Package Package { get; }

        public string Title => Package.GetAttribute("Title") ?? Package.Id;
        public string Author => Package.GetAttribute("Author") ?? "";
        public bool HasAuthor => Author != "";
        protected string rating;
        public string Rating {
            get
            {
                bool success = int.TryParse(rating, out int rint);
                if (!success) return "Unrated";
                else return $"{new string('★', rint)}{new string('☆', 5 - rint)}";
            }
        }
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

        public bool IsInstalled => Package.Token.State == ProgressToken.ProgressState.Installed;
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public PackageViewModel(Package package)
        {
            Package = package;
            rating = Package.GetAttribute("Rating") ?? "";

            Package.Token.PropertyChanged += Package_Token_PropertyChanged;
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

        private void Package_Token_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ProgressToken.State):
                    PropertyChanged?.Invoke(this, new(nameof(IsInstalled)));
                    break;
            }
        }
        #endregion
    }
}
