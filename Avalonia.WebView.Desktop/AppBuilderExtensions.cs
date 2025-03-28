using System.Runtime.InteropServices;
using Avalonia.WebView.Core.Configurations;
using Avalonia.WebView.Linux;
using Avalonia.WebView.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.WebView.Desktop;

public static class AppBuilderExtensions
{
    public static AppBuilder UseDesktopWebView(
        this AppBuilder builder,
        Func<WebViewCreationProperties>? configDelegate,
        bool isWslDevelop = false
    )
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            builder.UseWindowWebView(configDelegate);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            builder.UseLinuxWebView(configDelegate,isWslDevelop);

        return builder;
    }
    public static IServiceCollection AddDesktopWebView(this IServiceCollection services,Action<AppBuilder> appBuild)
    {
        return services;
    }
}
