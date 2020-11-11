using LibQSelect.PackageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QSelectAvalonia.ViewModels
{
    public class PackageWindowViewModel
    {
        #region Properties
        public Package Package { get; }

        public string Title => Package.Attributes["Title"] ?? Package.Id;
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
        public string Description => PreformatText(Package.Attributes["Title"] ?? "Unknown package.");
        public bool HasDependencies => Package.Dependencies != null;
        public bool IsDownloaded => Package.IsDownloaded;
        #endregion

        #region Constructors

        #endregion

        #region Methods
        protected string PreformatText(string text)
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
