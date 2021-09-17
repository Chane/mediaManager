using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.ReactiveUI;

namespace UI
{
    [ExcludeFromCodeCoverage(Justification = "Avalonia Scaffolding")]
    public class Program
    {
        public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        private static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI();
    }
}