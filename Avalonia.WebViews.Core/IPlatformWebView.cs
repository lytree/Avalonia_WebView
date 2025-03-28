using Avalonia.WebViews.Core.Shared;

namespace Avalonia.WebViews.Core;

public interface IPlatformWebView : IWebViewControl, IDisposable, IAsyncDisposable
{
    bool IsInitialized { get; }
    object? PlatformViewContext { get; }
    IntPtr NativeHandler { get; }
    Task<bool> Initialize();
}
