using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QSelectAvalonia.Pages
{
    public class HomePage : UserControl
    {
        protected TextBlock SplashTextBlock;

        public HomePage()
        {
            this.InitializeComponent();

            GetSplashTextAsync().ConfigureAwait(false);
        }

        protected async Task GetSplashTextAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = "QSelectAvalonia.Assets.splashes.txt";

            List<string> splashes;

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            using (StreamReader reader = new StreamReader(stream))
            {
                splashes = (await reader.ReadToEndAsync()).Split("\n").ToList();
            }

            Random random = new Random();
            SplashTextBlock.Text = splashes[random.Next(splashes.Count)];
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            SplashTextBlock = this.FindControl<TextBlock>("SplashTextBlock");
        }
    }
}
