using Avalonia.WebViews.Core.Enums;

namespace Avalonia.WebViews.Core.Events;

public class WebViewNewWindowEventArgs : EventArgs
{
    public Uri? Url { get; set; }
    public required UrlRequestStrategy UrlLoadingStrategy { get; set; }

    public object? RawArgs { get; set; }
}
