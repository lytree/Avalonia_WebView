using Avalonia.Controls;

namespace Avalonia.WebView.Core.Shared;

public interface IViewHandler
{
    Control AttachableControl { get; }
    IPlatformWebView PlatformWebView { get; }
}
