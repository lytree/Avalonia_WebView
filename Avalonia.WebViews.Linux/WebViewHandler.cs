using Avalonia.WebViews.Core;
using Avalonia.WebViews.Core.Configurations;
using Avalonia.WebViews.Core.Shared.Handlers;
using Avalonia.WebViews.Linux.Core;
using Avalonia.WebViews.Linux.Shared;

namespace Avalonia.WebViews.Linux;

public class WebViewHandler : ViewHandler<IVirtualWebView, LinuxWebViewCore>
{
    public WebViewHandler(
        ILinuxApplication linuxApplication,
        IVirtualWebView virtualWebView,
        IVirtualWebViewControlCallBack callback,
        IVirtualBlazorWebViewProvider? provider,
        WebViewCreationProperties webViewCreationProperties
    )
    {
        var webView = new LinuxWebViewCore(
            linuxApplication,
            this,
            callback,
            provider,
            webViewCreationProperties
        );
        _webViewCore = webView;
        PlatformWebView = webView;
        VirtualViewContext = virtualWebView;
        PlatformViewContext = webView;
    }

    readonly LinuxWebViewCore _webViewCore;

    protected override HandleRef CreatePlatformHandler(
        IPlatformHandle parent,
        Func<IPlatformHandle> createFromSystem
    )
    {
        //var handler = createFromSystem.Invoke();
        return new HandleRef(this, _webViewCore.NativeHandler);
    }

    protected override void Disposing()
    {
        PlatformWebView.Dispose();
        PlatformWebView = default!;
        VirtualViewContext = default!;
    }
}
