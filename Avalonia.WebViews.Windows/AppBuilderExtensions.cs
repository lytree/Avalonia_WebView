using Avalonia.WebViews.Core.Configurations;
using Avalonia.WebViews.Core.Extensions;
using Avalonia.WebViews.Core.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.WebViews.Windows;

public static class AppBuilderExtensions
{
    public static AppBuilder UseWindowWebView(this AppBuilder appBuilder, Func<WebViewCreationProperties>? config)
    {
        return appBuilder;
    }
}
