using System.Runtime.InteropServices;
using Avalonia.WebView.Core.Configurations;
using Avalonia.WebView.Windows;
using Splat;

namespace Avalonia.WebView.Desktop;

public static class AppBuilderExtensions
{
    public static AppBuilder UseDesktopWebView(
        this AppBuilder builder,
        Action<WebViewCreationProperties>? configDelegate,
        bool isWslDevelop = false
    )
    {
        WebViewCreationProperties creationProperties = new();
        configDelegate?.Invoke(creationProperties);
        Locator.CurrentMutable.RegisterConstant(creationProperties);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            builder.UseWindowWebView();
        // else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //     builder.UseMacCatalystWebView();
        // else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //     builder.UseLinuxWebView(isWslDevelop);

        return builder;
    }
}
