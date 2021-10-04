using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UI.Views
{
    public class MediaItem : UserControl
    {
        public MediaItem()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
