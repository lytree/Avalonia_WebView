using System.Runtime.InteropServices;
using Avalonia.WebViews.Core.Configurations;
using Avalonia.WebViews.Linux;
using Avalonia.WebViews.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.WebViews.Desktop;

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
            builder.UseLinuxWebView(configDelegate, isWslDevelop);

        return builder;
    }
    public static IServiceCollection AddDesktopWebView(this IServiceCollection services, Action<AppBuilder> appBuild)
    {
        return services;
    }
}
