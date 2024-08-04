using Avalonia.WebView.Core.Extensions;
using Avalonia.WebView.Core.Shared;
using Splat;

namespace Avalonia.WebView.Windows;

public static class AppBuilderExtensions
{
    public static AppBuilder UseWindowWebView(this AppBuilder appBuilder)
    {
        return appBuilder.AfterPlatformServicesSetup(_ =>
        {
            Locator.CurrentMutable.RegisterLazySingleton<
                IViewHandlerProvider,
                ViewHandlerProvider
            >();
        });
    }
}
