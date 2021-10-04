using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UI.ViewModels;

namespace UI.Views
{
    [ExcludeFromCodeCoverage(Justification = "Avalonia Scaffolding")]
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void Tree_OnClick(object? sender, RoutedEventArgs e)
        {
            var clickedDirectory = ((Tree)sender!).ClickedDirectory;

            var viewModel = (MainWindowViewModel)this.DataContext!;
            viewModel.SourceDirectory = clickedDirectory;

            await viewModel.OnClickCommand();
        }
    }
}
