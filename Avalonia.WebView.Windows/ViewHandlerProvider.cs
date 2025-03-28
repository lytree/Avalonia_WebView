using Avalonia.WebView.Core;
using Avalonia.WebView.Core.Configurations;
using Avalonia.WebView.Core.Shared;

namespace Avalonia.WebView.Windows;

internal class ViewHandlerProvider : IViewHandlerProvider
{
    IViewHandlerProvider IViewHandlerProvider.ViewHandlerProvider => this;

    IViewHandler IViewHandlerProvider.CreatePlatformWebViewHandler(
        IVirtualWebView virtualView,
        IVirtualWebViewControlCallBack virtualViewCallBack,
        IVirtualBlazorWebViewProvider? provider,
        Action<WebViewCreationProperties>? configDelegate
    )
    {
        var creatonProperty = new WebViewCreationProperties();
        configDelegate?.Invoke(creatonProperty);

        return new WebViewHandler(virtualView, virtualViewCallBack, provider, creatonProperty);
    }
}
