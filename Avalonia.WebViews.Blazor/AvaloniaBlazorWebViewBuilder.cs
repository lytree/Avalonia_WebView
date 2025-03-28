using Avalonia.WebViews.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.WebViews.Blazor;

public static class AvaloniaBlazorWebViewBuilder
{
    public static void Initialize(this IServiceCollection _serviceCollection, Action<WebViewCreationProperties>? webConfigDelegate, Action<BlazorWebViewSetting> configDelegate)
    {
        WebViewCreationProperties creationProperties = new();
        webConfigDelegate?.Invoke(creationProperties);
        _serviceCollection.AddSingleton<WebViewCreationProperties>(creationProperties);

        _serviceCollection.AddSingleton(new BlazorWebViewBuild(_serviceCollection, configDelegate));
    }
}