using Avalonia.WebView.Core.Configurations;

namespace Avalonia.WebView.Core.Shared;

public interface IViewHandlerProvider
{
    IViewHandler CreatePlatformWebViewHandler(
        IVirtualWebView virtualView,
        IVirtualWebViewControlCallBack virtualViewCallBack,
        IVirtualBlazorWebViewProvider? virtualBlazorWebViewCallBack,
        Action<WebViewCreationProperties>? configDelegate = default
    );

    public IViewHandlerProvider ViewHandlerProvider { get; }
}
