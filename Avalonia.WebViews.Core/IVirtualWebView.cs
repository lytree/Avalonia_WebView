namespace Avalonia.WebViews.Core;

public interface IVirtualWebView
{
    object VirtualViewObject { get; }
    IPlatformWebView? PlatformView { get; }
}
