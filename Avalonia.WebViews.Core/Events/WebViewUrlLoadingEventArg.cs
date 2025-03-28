namespace Avalonia.WebViews.Core.Events;

public class WebViewUrlLoadingEventArg : EventArgs
{
    public Uri? Url { get; set; }

    public bool Cancel { get; set; }

    public object? RawArgs { get; set; }
}
