namespace Avalonia.WebView.Core;

public interface IVirtualWebView<TVirtualView> : IVirtualWebView
{
    TVirtualView VirtualView { get; }
}
