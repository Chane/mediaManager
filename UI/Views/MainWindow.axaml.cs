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

        private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                var cb = (ComboBox)sender!;
                var cbi = (ComboBoxItem)cb.SelectedItem!;

                var viewModel = (MainWindowViewModel)this.DataContext;
                if (viewModel.Loaded)
                {
                    Enum.TryParse(cbi.Name, out SortBy sortBy);
                    viewModel.ApplyOrder(sortBy);
                }
            }
        }

        private async void Tree_OnClick(object? sender, RoutedEventArgs e)
        {
            var clickedDirectory = ((Tree)sender!).ClickedDirectory;

            var viewModel = (MainWindowViewModel)this.DataContext!;
            viewModel.SourceDirectory = clickedDirectory;

            await viewModel.Refresh();
        }

        private void Filter_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)this.DataContext!;
            viewModel.ApplyFilter();
        }
    }
}
