namespace Avalonia.WebViews.Linux.Shared;

public interface ILinuxApplication : IDisposable
{
    bool IsRunning { get; }
    ILinuxDispatcher Dispatcher { get; }
    Task<bool> RunAsync(string? applicationName, string[]? args);
    Task<(GWindow, WebKitWebView)> CreateWebView();
    // Task<(GWindow, WebKitWebView, IntPtr HostHandle)> CreateWebView();
    Task StopAsync();
}
