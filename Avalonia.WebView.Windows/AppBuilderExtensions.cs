using Avalonia.WebView.Core.Configurations;
using Avalonia.WebView.Core.Extensions;
using Avalonia.WebView.Core.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.WebView.Windows;

public static class AppBuilderExtensions
{
    public static AppBuilder UseWindowWebView(this AppBuilder appBuilder, Func<WebViewCreationProperties>? config)
    {
        return appBuilder;
    }
}
