using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using LibQuakePackageManager.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace QSelectAvalonia.Views
{
    public class PackageView : UserControl
    {
        public Package Package { get; protected set; }

        protected Expander PackageExpander;
        protected TextBlock HeaderLabel;
        protected Image PackageScreenshot;
        protected TextBlock DescriptionLabel;
        protected ListBox AttributesListBox;

        public PackageView()
        {
            this.InitializeComponent();
        }

        public PackageView(Package package) : this()
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));

            // Set header label
            if (package.Attributes.ContainsKey("Title")) HeaderLabel.Text = package.Attributes["Title"];
            else HeaderLabel.Text = package.Id;

            // Set description label
            if (package.Attributes.ContainsKey("Description")) DescriptionLabel.Text = package.Attributes["Description"];
            else DescriptionLabel.Text = "Unknown package.";

            // Set attributes list box
            List<string> attributes = package.Attributes.Where(x => x.Key != "Title" && x.Key != "Screenshot" && x.Key != "Description").Select(x => $"{x.Key}: {x.Value}").ToList();
            AttributesListBox.Items = attributes;

            PackageExpander.GotFocus += (a, b) => LoadImageAsync().ConfigureAwait(false);
        }

        public async Task LoadImageAsync()
        {
            if (PackageScreenshot.Source == null)
            {
                // Set package screenshot
                if (Package.Attributes.ContainsKey("Screenshot"))
                {
                    byte[] data;
                    using (WebClient client = new WebClient())
                    {
                        data = await client.DownloadDataTaskAsync(Package.Attributes["Screenshot"]);
                    }
                    PackageScreenshot.Source = new Bitmap(new MemoryStream(data));
                }
            }
        }


        public void SetItem(object item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            if (item is Package package)
            {
                Package = package;

                // Set header label
                if (package.Attributes.ContainsKey("Title")) HeaderLabel.Text = package.Attributes["Title"];
                else HeaderLabel.Text = package.Id;

                // Set description label
                if (package.Attributes.ContainsKey("Description")) DescriptionLabel.Text = package.Attributes["Description"];
                else DescriptionLabel.Text = "Unknown package.";

                // Set attributes list box
                List<string> attributes = package.Attributes.Where(x => x.Key != "Title" && x.Key != "Screenshot" && x.Key != "Description").Select(x => $"{x.Key}: {x.Value}").ToList();
                AttributesListBox.Items = attributes;
            }
            else
            {
                throw new ArgumentException("Provided item was not a Package!");
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            PackageExpander = this.FindControl<Expander>("PackageExpander");
            HeaderLabel = this.FindControl<TextBlock>("HeaderLabel");
            PackageScreenshot = this.FindControl<Image>("PackageScreenshot");
            DescriptionLabel = this.FindControl<TextBlock>("DescriptionLabel");
            AttributesListBox = this.FindControl<ListBox>("AttributesListBox");

            PackageExpander.GotFocus += (a, b) => LoadImageAsync().ConfigureAwait(false);
        }
    }
}
