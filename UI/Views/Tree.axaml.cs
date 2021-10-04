using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using UI.ViewModels;

namespace UI.Views
{
    [ExcludeFromCodeCoverage]
    public class Tree : ReactiveUserControl<TreeNodeViewModel>
    {
        private static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
            RoutedEvent.Register<Tree, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        public Tree()
        {
            this.InitializeComponent();
        }

        public string ClickedDirectory { get; private set; } = string.Empty;

        public event EventHandler<RoutedEventArgs> Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var directory = ((TreeNodeViewModel)((Button)sender).DataContext!).Directory;

            this.ClickedDirectory = directory;

            var eventArgs = new RoutedEventArgs { RoutedEvent = ClickEvent };
            this.RaiseEvent(eventArgs);
        }
    }
}
