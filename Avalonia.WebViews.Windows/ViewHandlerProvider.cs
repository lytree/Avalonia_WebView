using Avalonia.WebViews.Core;
using Avalonia.WebViews.Core.Configurations;
using Avalonia.WebViews.Core.Shared;

namespace Avalonia.WebViews.Windows;

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
