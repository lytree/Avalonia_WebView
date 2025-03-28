using Avalonia.Controls;

namespace Avalonia.WebViews.Core.Shared;

public interface IViewHandler
{
    Control AttachableControl { get; }
    IPlatformWebView PlatformWebView { get; }
}
