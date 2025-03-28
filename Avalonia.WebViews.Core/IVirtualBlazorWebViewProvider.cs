using Avalonia.WebViews.Core.Events;
using Avalonia.WebViews.Core.Models;

namespace Avalonia.WebViews.Core;

public interface IVirtualBlazorWebViewProvider
{
    Uri BaseUri { get; }

    bool ResourceRequestedFilterProvider(object? requester, out WebScheme filter);

    bool PlatformWebViewResourceRequested(
        object? sender,
        WebResourceRequest request,
        out WebResourceResponse? response
    );

    void PlatformWebViewMessageReceived(object? sender, WebViewMessageReceivedEventArgs arg);
}
