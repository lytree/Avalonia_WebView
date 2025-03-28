namespace Avalonia.WebViews.Core;

public interface IVirtualWebView<TVirtualView> : IVirtualWebView
{
    TVirtualView VirtualView { get; }
}
