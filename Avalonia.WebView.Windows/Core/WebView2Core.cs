using Avalonia.WebView.Core;
using Avalonia.WebView.Core.Configurations;
using Avalonia.WebView.Core.Shared.Handlers;

namespace Avalonia.WebView.Windows.Core;

public sealed partial class WebView2Core : IPlatformWebView<WebView2Core>
{
    public WebView2Core(
        ViewHandler handler,
        IVirtualWebViewControlCallBack callback,
        IVirtualBlazorWebViewProvider? provider,
        WebViewCreationProperties webViewCreationProperties
    )
    {
        _hwndTaskSource = new();
        _callBack = callback;
        _handler = handler;
        _creationProperties = webViewCreationProperties;
        _provider = provider;

        if (handler.RefHandler.Handle != IntPtr.Zero)
        {
            NativeHandler = handler.RefHandler.Handle;
            _hwndTaskSource.SetResult(handler.RefHandler.Handle);
        }

        SetEnvironmentDefaultBackground(webViewCreationProperties.DefaultWebViewBackgroundColor);
        RegisterEvents();
    }

    ~WebView2Core()
    {
        Dispose(disposing: false);
    }

    private readonly IVirtualBlazorWebViewProvider? _provider;
    private readonly IVirtualWebViewControlCallBack _callBack;
    private readonly ViewHandler _handler;
    private readonly WebViewCreationProperties _creationProperties;
    private readonly TaskCompletionSource<IntPtr> _hwndTaskSource;

    private bool _browserHitTransparent;

    private bool _isInitialized;
    public bool IsInitialized
    {
        get => Volatile.Read(ref _isInitialized);
        private set => Volatile.Write(ref _isInitialized, value);
    }

    private bool _isDisposed = false;
    public bool IsDisposed
    {
        get => Volatile.Read(ref _isDisposed);
        private set => Volatile.Write(ref _isDisposed, value);
    }

    private CoreWebView2Environment? _coreWebView2Environment { get; set; }
    private CoreWebView2Controller? _coreWebView2Controller { get; set; }
    private CoreWebView2ControllerOptions? _controllerOptions { get; set; }

    [Browsable(false)]
    public CoreWebView2? CoreWebView2
    {
        get
        {
            VerifyNotDisposed();
            // VerifyBrowserNotCrashed();
            return _coreWebView2Controller?.CoreWebView2;
        }
    }
}
