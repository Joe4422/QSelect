using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace QSelectAvalonia.Controls
{
    public class FormattedTextBlock : UserControl
    {
        protected StackPanel TextStackPanel;
        protected List<TextBlock> TextBlockList;

        protected string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                SetText(value);
            }
        }

        public FormattedTextBlock()
        {
            this.InitializeComponent();

            TextStackPanel = new StackPanel();
            TextBlockList = new List<TextBlock>();

            this.Content = TextStackPanel;
        }

        protected void SetText(string text)
        {
            TextBlockList.Clear();
            TextStackPanel.Children.Clear();

            text = PreformatText(text);

            TextBlockList.Add(new TextBlock() { Text = text, TextWrapping = Avalonia.Media.TextWrapping.Wrap });

            foreach (TextBlock tb in TextBlockList)
            {
                TextStackPanel.Children.Add(tb);
            }
        }

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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
