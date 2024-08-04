namespace Avalonia.WebView.Core;

public interface IVirtualWebViewProvider
{
    string AppHostAddress { get; }
    Uri BaseUri { get; }
}
