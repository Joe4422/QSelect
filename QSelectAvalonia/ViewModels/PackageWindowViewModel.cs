using Avalonia.Media;
using Avalonia.Media.Imaging;
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

namespace QSelectAvalonia.ViewModels
{
    public class PackageWindowViewModel : INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// The package this PackageWindowViewModel applies to.
        /// </summary>
        public Package Package { get; }

        /// <summary>
        /// Package's title.
        /// </summary>
        public string Title => Package.Attributes["Title"] ?? Package.Id;
        /// <summary>
        /// Package's attributes, arranged into a list of strings.
        /// </summary>
        public List<string> Attributes
        {
            get
            {
                List<string> atts = Package.Attributes.Where(x => x.Key != "Title" && x.Key != "Description" && x.Key != "Screenshot").Select(x => $"{x.Key}: {x.Value}").ToList();
                atts.Add($"ID: {Package.Id}");
                if (Package.IsDownloaded) atts.Add($"Path: {Package.InstallPath}");

                return atts;
            }
        }
        /// <summary>
        /// Package's Description attribute, or "Unknown package." if not present.
        /// </summary>
        public string Description => PreformatText(Package.Attributes["Description"] ?? "Unknown package.");
        /// <summary>
        /// True if the package has dependencies, false otherwise.
        /// </summary>
        public bool HasDependencies => Package.Dependencies != null;
        /// <summary>
        /// Whether the package has been downloaded.
        /// </summary>
        public bool IsDownloaded => Package.IsDownloaded;
        /// <summary>
        /// Whether the package has not been downloaded.
        /// </summary>
        public bool IsNotDownloaded => !Package.IsDownloaded;
        /// <summary>
        /// Whether the package can be played, i.e. the game is not currently running and the package has been downloaded.
        /// </summary>
        public bool PackageIsPlayable => !GameService.Game.GameRunning && Package.IsDownloaded && GameService.Game?.LoadedSourcePort != null;
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

            package.PropertyChanged += ModelPropertyChanged;
            GameService.Game.PropertyChanged += ModelPropertyChanged;
        }
        #endregion

        #region Methods
        protected async Task LoadScreenshotAsync()
        {
            if (Package.HasAttribute("ScreenshotURL"))
            {
                Screenshot = await PackageImageService.GetScreenshotAsync(Package);
            }
        }

        private void GameService_Initialised()
        {
            GameService.Game.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Package.IsDownloaded):
                    PropertyChanged?.Invoke(this, new(nameof(IsDownloaded)));
                    PropertyChanged?.Invoke(this, new(nameof(IsNotDownloaded)));
                    PropertyChanged?.Invoke(this, new(nameof(PackageIsPlayable)));
                    break;
                case nameof(GameManager.GameRunning):
                    PropertyChanged?.Invoke(this, new(nameof(PackageIsPlayable)));
                    break;
                case nameof(GameManager.LoadedSourcePort):
                    PropertyChanged?.Invoke(this, new(nameof(PlayNowString)));
                    PropertyChanged?.Invoke(this, new(nameof(PackageIsPlayable)));
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
