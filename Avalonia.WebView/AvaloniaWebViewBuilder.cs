using Avalonia.WebView.Core.Configurations;
using Splat;

namespace Avalonia.WebView;

public static class AvaloniaWebViewBuilder
{
    public static void Initialize(Action<WebViewCreationProperties>? configDelegate)
    {
        WebViewCreationProperties creationProperties = new();
        configDelegate?.Invoke(creationProperties);
        Locator.CurrentMutable.RegisterConstant(creationProperties);
    }
}
