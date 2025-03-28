using Avalonia.WebView.Core;
using Avalonia.WebView.Core.Events;
using Avalonia.WebView.Core.Extensions;
using Avalonia.WebView.Core.Helpers;
using Avalonia.WebView.Core.Models;
using Avalonia.WebView.Linux.Shared;
using Gio;
using Task = System.Threading.Tasks.Task;

namespace Avalonia.WebView.Linux.Core;

partial class LinuxWebViewCore
{
    Task PrepareBlazorWebViewStarting(
        IVirtualBlazorWebViewProvider? provider,
        WebKitWebView webView
    )
    {
        if (provider is null || WebView is null)
            return Task.CompletedTask;

        if (!provider.ResourceRequestedFilterProvider(this, out var filter))
            return Task.CompletedTask;

        _webScheme = filter;
        var bRet = _dispatcher
            .InvokeAsync(() =>
            {
                //webView.Context.RegisterUriScheme("app", WebView_WebResourceRequest);
                webView.WebContext.RegisterUriScheme(filter.Scheme, WebView_WebResourceRequest);

                var userContentManager = webView.GetUserContentManager();

                var script = UserScript.New(BlazorScriptHelper.BlazorStartingScript, WebKit.UserContentInjectedFrames.AllFrames,
                                                           WebKit.UserScriptInjectionTime.Start,
                                                           null, null);
                userContentManager.AddScript(script);
                script.Unref();

                // GObject.Functions.SignalConnectCon(
                //     userContentManager.Handle,
                //     $"script-message-received::{_messageKeyWord}",
                //     LinuxApplicationManager.LoadFunction(_userContentMessageReceived),
                //     IntPtr.Zero
                // );
                userContentManager.RegisterScriptMessageHandler(userContentManager.Handle.ToString(), _messageKeyWord);
            })
            .Result;

        _isBlazorWebView = true;
        return Task.CompletedTask;
    }

    void WebView_WebMessageReceived(UserContentManager contentManager, UserScript pJsResult, nint pArg)
    {
        if (_provider is null)
            return;

        var pJsStringValue =  _webView.EvaluateJavascriptAsync(pJsResult.ToString()).Result;
        if (!pJsStringValue.IsString())
            return;

        var message = new WebViewMessageReceivedEventArgs
        {
            Message = pJsStringValue.ToString(),
            Source = _provider.BaseUri,
        };
        pJsResult.Unref();

        _callBack.PlatformWebViewMessageReceived(this, message);
        _provider?.PlatformWebViewMessageReceived(this, message);
    }

    private void WebView_WebResourceRequest(URISchemeRequest request)
    {
        if (_provider is null)
            return;

        if (_webScheme is null)
            return;

        if (request.GetScheme() != _webScheme.Scheme)
            return;

        //bool allowFallbackOnHostPage = request.Path == "/";
        var allowFallbackOnHostPage = _webScheme.BaseUri.IsBaseOfPage(request.GetUri());
        var requestWrapper = new WebResourceRequest
        {
            RequestUri = request.GetUri(),
            AllowFallbackOnHostPage = allowFallbackOnHostPage,
        };

        var bRet = _provider.PlatformWebViewResourceRequested(
            this,
            requestWrapper,
            out var response
        );
        if (!bRet)
            return;

        if (response is null)
            return;

        var headerString = response.Headers[QueryStringHelper.ContentTypeKey];
        using var ms = new MemoryStream();
        response.Content.CopyTo(ms);
        var stream = MemoryInputStream.NewFromBytes(GLib.Bytes.New(ms.GetBuffer()));
        request.Finish(stream, ms.Length, headerString);
    }
}
