using Avalonia.WebView.Core.Extensions;
using Avalonia.WebView.Core.Shared;
using Avalonia.WebView.Linux.Shared;
using Splat;

namespace Avalonia.WebView.Linux;

public static class AppBuilderExtensions
{
    public static AppBuilder UseLinuxWebView(this AppBuilder builder, bool isWslDevelop)
    {
        return builder.AfterPlatformServicesSetup(_ =>
        {
            Locator.CurrentMutable.RegisterLazySingleton(
                () => LinuxApplicationBuilder.Build(isWslDevelop)
            );
            Locator.CurrentMutable.RegisterLazySingleton<
                IViewHandlerProvider,
                ViewHandlerProvider
            >();
        });
    }
}
