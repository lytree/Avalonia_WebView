using Avalonia.WebView.Linux.Shared.Extensions;
using Gdk;
using Gio;
using System;
using WebKit;
using Task = System.Threading.Tasks.Task;

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

    private const string DevToysInteropName = "devtoyswebinterop";
    private const string Scheme = "app";
    internal const string AppHostAddress = "localhost";
    internal static readonly Uri AppOriginUri = new($"{Scheme}://{AppHostAddress}/");

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

        try
        {
            _application = GApplication.New(null, Gio.ApplicationFlags.NonUnique);


            GLib.Functions.SetPrgname("DevToys");
            // Set the human-readable application name for app bar and task list.
            GLib.Functions.SetApplicationName("DevToys");

            _application.OnActivate += OnApplicationActivate;
            _application.OnShutdown += OnApplicationShutdown;

            WebKit.Module.Initialize();
            // _application.Register(GLib.Cancellable.Current);
            _dispatcher.Start();

            _defaultDisplay = GDisplay.GetDefault();
            IsRunning = true;
            taskSource.SetResult(true);
            // _application.();
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
            GWindow window = Gtk.ApplicationWindow.New(_application); ;
            _application?.AddWindow(window);
            WebKitWebView webView = CreateWebView();
            window.SetChild(webView);
            window.Show();
            return (window, webView, window.X11Handle());
        });
    }


    private WebKitWebView CreateWebView()
    {
        var webView = new WebKitWebView();


        // Make web view transparent
        webView.SetBackgroundColor(new RGBA { Red = 255, Blue = 0, Green = 0, Alpha = 0 });

        // Initialize some basic properties of the WebView
        Settings webViewSettings = webView.GetSettings();
        webViewSettings.EnableDeveloperExtras = true;
        webViewSettings.JavascriptCanAccessClipboard = true;
        webViewSettings.EnableBackForwardNavigationGestures = false;
        webViewSettings.MediaPlaybackRequiresUserGesture = false;
        webViewSettings.HardwareAccelerationPolicy = HardwareAccelerationPolicy.Never; // https://github.com/DevToys-app/DevToys/issues/1234
        webView.SetSettings(webViewSettings);

        UserContentManager userContentManager = webView.GetUserContentManager();

        // Handle messages.
        UserContentManager.ScriptMessageReceivedSignal.Connect(
            userContentManager,
            HandleScriptMessageReceivedSignal,
            after: false,
            detail: DevToysInteropName);
        if (!userContentManager.RegisterScriptMessageHandler(DevToysInteropName, null))
        {
            throw new Exception("Could not register script message handler");
        }

        // Add Blazor initialization script.
        //userContentManager.AddScript(
        //    UserScript.New(
        //        BlazorInitScript,
        //        injectedFrames: UserContentInjectedFrames.AllFrames,
        //        injectionTime: UserScriptInjectionTime.End,
        //        allowList: null,
        //        blockList: null));

        // Register a "app" url scheme to handle Blazor resources
        webView.WebContext.RegisterUriScheme(Scheme, HandleUriScheme);

        return webView;
    }
    private void HandleUriScheme(URISchemeRequest request)
    {
        _appSchemeHandler.StartUrlSchemeTask(request);
    }


    private void OnApplicationActivate(object sender, object e)
    {

    }

    private void OnApplicationShutdown(object sender, object e)
    {

        _application.OnActivate -= OnApplicationActivate;
        _application.OnShutdown -= OnApplicationShutdown;
    }
}
