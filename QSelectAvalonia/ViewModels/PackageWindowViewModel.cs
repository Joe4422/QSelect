using Avalonia.Media;
using Avalonia.Media.Imaging;
using LibPackageManager.Managers;
using LibQSelect;
using LibQSelect.PackageManager.Packages;
using QSelectAvalonia.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibPackageManager.Repositories;

namespace QSelectAvalonia.ViewModels
{
    public class PackageWindowViewModel : INotifyPropertyChanged
    {
        #region Variables
        protected AcquireItemJob<Package> job = null;
        #endregion

        #region Properties
        /// <summary>
        /// The package this PackageWindowViewModel applies to.
        /// </summary>
        public Package Package { get; }

        /// <summary>
        /// Package's title.
        /// </summary>
        public string Title => Package.GetAttribute("Title") ?? Package.Id;
        /// <summary>
        /// Package's attributes, arranged into a list of strings.
        /// </summary>
        public List<string> Attributes
        {
            get
            {
                List<string> atts = Package.Attributes.Where(x => x.Key != "Title" && x.Key != "Description" && x.Key != "ScreenshotURL" && x.Key != "ThumbnailURL" && x.Key != "StartMaps").Select(x => $"{x.Key}: {x.Value}").ToList();
                atts.Add($"ID: {Package.Id}");
                return atts;
            }
        }
        /// <summary>
        /// Package's Description attribute, or "Unknown package." if not present.
        /// </summary>
        public string Description => PreformatText(Package.GetAttribute("Description") ?? "Unknown package.");
        /// <summary>
        /// True if the package has dependencies, false otherwise.
        /// </summary>
        public bool HasDependencies => Package.Dependencies != null;
        /// <summary>
        /// Whether the package has been downloaded.
        /// </summary>
        public bool IsInstalled => Package.Token.State is ProgressToken.ProgressState.Installed;
        /// <summary>
        /// Whether a download can be initiated on the package.
        /// </summary>
        public bool CanBeInstalled => Package.Token.State is ProgressToken.ProgressState.NotStarted or ProgressToken.ProgressState.Failed;
        /// <summary>
        /// Whether the package can be played, i.e. the game is not currently running and the package has been downloaded.
        /// </summary>
        public bool CanBePlayed => !GameService.Game.GameRunning && IsInstalled && GameService.Game?.LoadedSourcePort != null;
        /// <summary>
        /// Whether the download progress bar is visible.
        /// </summary>
        public bool ProgressBarVisible => Package.Token.State is not ProgressToken.ProgressState.NotStarted and not ProgressToken.ProgressState.Installed;
        public string ProgressBarText
        {
            get
            {
                if (Package.Token.State == ProgressToken.ProgressState.DownloadInProgress)
                {
                    return $"Downloading... ({DownloadBarProgress:##}%)";
                }
                else if (Package.Token.State == ProgressToken.ProgressState.InstallInProgress)
                {
                    return $"Installing...";
                }
                else if (Package.Token.State == ProgressToken.ProgressState.Failed)
                {
                    return "Failed!";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// How far along the download is.
        /// </summary>
        private float downloadBarProgress = 0.0f; public float DownloadBarProgress
        {
            get => downloadBarProgress;
            set
            {
                downloadBarProgress = value;
                PropertyChanged?.Invoke(this, new(nameof(DownloadBarProgress)));
            }
        }

        /// <summary>
        /// The package screenshot bitmap.
        /// </summary>
        private IImage screenshot = null; public IImage Screenshot
        {
            get => screenshot;
            set
            {
                screenshot = value;
                PropertyChanged?.Invoke(this, new(nameof(Screenshot)));
            }
        }
        /// <summary>
        /// List of package dependencies.
        /// </summary>
        public List<string> Dependencies
        {
            get
            {
                List<string> deps = new();
                foreach (var kvp in Package.Dependencies)
                {
                    if (kvp.Value == null || (kvp.Value as Package).HasAttribute("Title") == false) deps.Add(kvp.Key);
                    else deps.Add((kvp.Value as Package).Attributes["Title"]);
                }
                return deps;
            }
        }
        /// <summary>
        /// String for Play Now button.
        /// </summary>
        public string PlayNowString {
            get
            {
                if (GameService.Game.LoadedSourcePort == null) return "Play now";
                else return $"Play now in {GameService.Game.LoadedSourcePort.Name}";
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public PackageWindowViewModel(Package package)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));

            LoadScreenshotAsync().ConfigureAwait(false);

            package.Token.PropertyChanged += ModelPropertyChanged;
            GameService.Game.PropertyChanged += ModelPropertyChanged;
            DownloadService.Packages.DownloadJobStarted += Packages_DownloadJobStarted;
        }
        #endregion

        #region Methods
        private void Packages_DownloadJobStarted(object sender, AcquireItemJob<Package> job)
        {
            if (job.MainItem == Package)
            {
                this.job = job;
                foreach (Package package in job.ItemsToAcquire)
                {
                    if (package == Package) continue;

                    package.Token.PropertyChanged += ModelPropertyChanged;
                }
            }
        }

        protected async Task LoadScreenshotAsync()
        {
            if (Package.HasAttribute("ScreenshotURL"))
            {
                Screenshot = await PackageImageService.GetScreenshotAsync(Package);
            }
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ProgressToken.State):
                    PropertyChanged?.Invoke(this, new(nameof(IsInstalled)));
                    PropertyChanged?.Invoke(this, new(nameof(CanBeInstalled)));
                    PropertyChanged?.Invoke(this, new(nameof(CanBePlayed)));
                    PropertyChanged?.Invoke(this, new(nameof(ProgressBarVisible)));
                    PropertyChanged?.Invoke(this, new(nameof(ProgressBarText)));
                    break;
                case nameof(GameLauncher.GameRunning):
                    PropertyChanged?.Invoke(this, new(nameof(CanBePlayed)));
                    break;
                case nameof(GameLauncher.LoadedSourcePort):
                    PropertyChanged?.Invoke(this, new(nameof(PlayNowString)));
                    PropertyChanged?.Invoke(this, new(nameof(CanBePlayed)));
                    break;
                case nameof(ProgressToken.DownloadPercentage):
                    int maxProgress = job.ItemsToAcquire.Count * 100;
                    int currentProgress = job.ItemsToAcquire.Sum(x => x.Token.DownloadPercentage);
                    DownloadBarProgress = ((float)currentProgress / (float)maxProgress) * 100.0f;
                    PropertyChanged?.Invoke(this, new(nameof(CanBeInstalled)));
                    PropertyChanged?.Invoke(this, new(nameof(ProgressBarVisible)));
                    PropertyChanged?.Invoke(this, new(nameof(ProgressBarText)));
                    break;
            }
        }

        protected static string PreformatText(string text)
        {
            string s = text;

            s = s.Replace("<br/>", "\n");
            s = s.Replace("<br />", "\n");
            s = s.Replace("<li>", " •   ");
            s = s.Replace("</li>", "\n");
            s = s.Replace("<ul>", "\n");
            s = s.Replace("</ul>", "\n");
            s = s.Replace("<ol>", "\n");
            s = s.Replace("</ol>", "\n");

            s = Regex.Replace(s, @"\<.+?\>", "");

            return s;
        }
        #endregion
    }
}
