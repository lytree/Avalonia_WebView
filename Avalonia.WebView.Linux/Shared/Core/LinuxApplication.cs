using Avalonia.WebView.Linux.Shared.Extensions;
using Avalonia.WebView.Linux.Shared.Interoperates;

namespace Avalonia.WebView.Linux.Shared.Core;

internal class LinuxApplication : ILinuxApplication
{
    public LinuxApplication(bool isWslDevelop)
    {
        _isWslDevelop = isWslDevelop;
        _dispatcher = new LinuxDispatcher();
    }

    ~LinuxApplication()
    {
        Dispose(disposing: false);
    }

    private readonly bool _isWslDevelop;
    private readonly ILinuxDispatcher _dispatcher;

    //Task? _appRunning;
    private Thread? _appThread;
    private GDisplay? _defaultDisplay;
    private GApplication? _application;

    private bool _isRunning;

    public bool IsRunning
    {
        get => _isRunning;
        protected set => _isRunning = value;
    }

    private bool _isDisposed;

    public bool IsDisposed
    {
        get => _isDisposed;
        protected set => _isDisposed = value;
    }

    bool ILinuxApplication.IsRunning => IsRunning;

    ILinuxDispatcher ILinuxApplication.Dispatcher => _dispatcher;

    Task<bool> ILinuxApplication.RunAsync(string? applicationName, string[]? args)
    {
        if (IsRunning)
            return Task.FromResult(true);

        var tcs = new TaskCompletionSource<bool>();
        //_appRunning = Task.Factory.StartNew(obj =>
        //{
        //    Environment.SetEnvironmentVariable("WAYLAND_DISPLAY", "/proc/fake-display-to-prevent-wayland-initialization-by-gtk3");
        //    if (!_isWslDevelop)
        //        GtkApi.SetAllowedBackends("x11,wayland,quartz,*");
        //        //GtkApi.SetAllowedBackends("x11");
        //    GApplication.Init();
        //    _defaultDisplay = GDisplay.Default;
        //
        //    _application = new("WebView.Application", GLib.ApplicationFlags.None);
        //    _application.Register(GLib.Cancellable.Current);
        //
        //    _dispatcher.Start();
        //    IsRunning = true;
        //
        //    tcs.SetResult(true);
        //    GApplication.Run();
        //}, TaskCreationOptions.LongRunning);
        //

        _appThread = new Thread(() => Run(tcs))
        {
            Name = "GTK3WORKINGTHREAD",
            IsBackground = true,
        };
        _appThread.Start();

        return tcs.Task;
    }

    private void Run(TaskCompletionSource<bool> taskSource)
    {
        if (!_isWslDevelop)
            GtkApi.SetAllowedBackends("x11");
        //GtkApi.SetAllowedBackends("x11,wayland,quartz,*");
        Environment.SetEnvironmentVariable(
            "WAYLAND_DISPLAY",
            "/proc/fake-display-to-prevent-wayland-initialization-by-gtk3"
        );

        try
        {
            GApplication.Init();
            _application = new("WebView.Application", GLib.ApplicationFlags.None);
            _application.Register(GLib.Cancellable.Current);
            _dispatcher.Start();

            _defaultDisplay = GDisplay.Default;
            IsRunning = true;
            taskSource.SetResult(true);
            GApplication.Run();
        }
        catch
        {
            taskSource.SetResult(false);
        }
    }

    Task ILinuxApplication.StopAsync()
    {
        if (!IsRunning)
            return Task.CompletedTask;

        _application = null;
        _dispatcher.Stop();
        GApplication.Quit();
        _appThread?.Join();
        //_appRunning?.Wait();
        return Task.CompletedTask;
    }

    protected virtual async void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing) { }

            await ((ILinuxApplication)this).StopAsync();

            _defaultDisplay?.Dispose();
            _defaultDisplay = null;

            IsDisposed = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    Task<(GWindow, WebKitWebView, IntPtr HostHandle)> ILinuxApplication.CreateWebView()
    {
        if (!_isRunning)
            throw new InvalidOperationException(nameof(IsRunning));
        return _dispatcher.InvokeAsync(() =>
        {
            GWindow window = new("WebView.Gtk.Window");
            _application?.AddWindow(window);
            window.KeepAbove = true;
            WebKitWebView webView = new(new Settings { EnableFullscreen = true });
            window.Add(webView);
            window.ShowAll();
            window.Present();
            return (window, webView, window.X11Handle());
        });
    }
}
