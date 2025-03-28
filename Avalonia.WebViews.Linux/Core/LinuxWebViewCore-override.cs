using Avalonia.WebViews.Core;
using Avalonia.WebViews.Core.Events;

namespace Avalonia.WebViews.Linux.Core;

partial class LinuxWebViewCore
{
    // public IntPtr NativeHandler { get; }
    LinuxWebViewCore IPlatformWebView<LinuxWebViewCore>.PlatformView => this;

    bool IPlatformWebView.IsInitialized => IsInitialized;

    object IPlatformWebView.PlatformViewContext => this;

    bool IWebViewControl.IsCanGoForward => _dispatcher.InvokeAsync(WebView.CanGoForward).Result;

    bool IWebViewControl.IsCanGoBack => _dispatcher.InvokeAsync(WebView.CanGoBack).Result;

    async Task<bool> IPlatformWebView.Initialize()
    {
        if (IsInitialized)
            return true;

        try
        {
            _ = _dispatcher
                .InvokeAsync(() =>
                {
                    var settings = WebView.GetSettings();
                    settings.EnableDeveloperExtras = _creationProperties.AreDevToolEnabled;
                    settings.AllowFileAccessFromFileUrls = true;
                    settings.AllowModalDialogs = true;
                    settings.AllowTopNavigationToDataUrls = true;
                    settings.AllowUniversalAccessFromFileUrls = true;
                    settings.EnableBackForwardNavigationGestures = true;
                    settings.EnableCaretBrowsing = false;
                    settings.EnableMediaCapabilities = true;
                    settings.EnableMediaStream = true;
                    settings.JavascriptCanAccessClipboard = true;
                    settings.JavascriptCanOpenWindowsAutomatically = true;
                    WebView.SetSettings(settings);
                })
                .Result;

            RegisterWebViewEvents(WebView);

            await PrepareBlazorWebViewStarting(_provider, WebView);

            IsInitialized = true;
            _callBack.PlatformWebViewCreated(
                this,
                new WebViewCreatedEventArgs { IsSucceed = true }
            );

            return true;
        }
        catch (Exception ex)
        {
            _callBack.PlatformWebViewCreated(
                this,
                new WebViewCreatedEventArgs { IsSucceed = false, Message = ex.ToString() }
            );
        }

        return false;
    }

    Task<string?> IWebViewControl.ExecuteScriptAsync(string javaScript)
    {
        if (string.IsNullOrWhiteSpace(javaScript))
            return Task.FromResult<string?>(default);

        var messageJsStringLiteral = HttpUtility.JavaScriptStringEncode(javaScript);
        var script = $"{_dispatchMessageCallback}((\"{messageJsStringLiteral}\"))";

        _ = _dispatcher
            .InvokeAsync(() =>
            {
                WebView.EvaluateJavascriptAsync(script);
            })
            .Result;

        return Task.FromResult<string?>(string.Empty);
    }

    bool IWebViewControl.GoBack()
    {
        return _dispatcher
            .InvokeAsync(() =>
            {
                if (!WebView.CanGoBack())
                    return false;

                WebView.GoBack();
                return true;
            })
            .Result;
    }

    bool IWebViewControl.GoForward()
    {
        return _dispatcher
            .InvokeAsync(() =>
            {
                if (!WebView.CanGoForward())
                    return false;

                WebView.GoForward();
                return true;
            })
            .Result;
    }

    bool IWebViewControl.Navigate(Uri? uri)
    {
        if (uri is null)
            return false;

        return _dispatcher.InvokeAsync(() => WebView.LoadUri(uri.AbsoluteUri)).Result;
    }

    bool IWebViewControl.NavigateToString(string htmlContent)
    {
        if (string.IsNullOrWhiteSpace(htmlContent))
            return false;

        return _dispatcher.InvokeAsync(() => WebView.LoadHtml(htmlContent, null)).Result;
    }

    bool IWebViewControl.OpenDevToolsWindow()
    {
        return false;
    }

    bool IWebViewControl.PostWebMessageAsJson(string webMessageAsJson, Uri? baseUri)
    {
        if (string.IsNullOrWhiteSpace(webMessageAsJson))
            return false;

        var messageJsStringLiteral = HttpUtility.JavaScriptStringEncode(webMessageAsJson);
        var script = $"{_dispatchMessageCallback}((\"{messageJsStringLiteral}\"))";

        return _dispatcher
            .InvokeAsync(() =>
            {
                WebView.EvaluateJavascriptAsync(script);
            })
            .Result;
    }

    bool IWebViewControl.PostWebMessageAsString(string webMessageAsString, Uri? baseUri)
    {
        if (string.IsNullOrWhiteSpace(webMessageAsString))
            return false;

        var messageJsStringLiteral = HttpUtility.JavaScriptStringEncode(webMessageAsString);
        var script = $"{_dispatchMessageCallback}((\"{messageJsStringLiteral}\"))";

        return _dispatcher
            .InvokeAsync(() =>
            {
                WebView.EvaluateJavascriptAsync(script);
            })
            .Result;
    }

    bool IWebViewControl.Reload() => _dispatcher.InvokeAsync(WebView.Reload).Result;

    bool IWebViewControl.Stop() => _dispatcher.InvokeAsync(WebView.StopLoading).Result;

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                try
                {
                    UnregisterWebViewEvents(WebView);
                    UnregisterEvents();

                    _ = _dispatcher
                        .InvokeAsync(() =>
                        {
                            WebView.Dispose();
                            _hostWindow.Dispose();
                        })
                        .Result;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            IsDisposed = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        ((IDisposable)this).Dispose();
        return new ValueTask();
    }
}
