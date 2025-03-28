namespace Avalonia.WebViews.Core;

public interface IVirtualWebViewProvider
{
    string AppHostAddress { get; }
    Uri BaseUri { get; }
}
