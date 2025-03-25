﻿using Avalonia.WebView.Core;
using Avalonia.WebView.Core.Events;
using Avalonia.WebView.Core.Extensions;
using Avalonia.WebView.Core.Helpers;
using Avalonia.WebView.Core.Models;
using Avalonia.WebView.Linux.Shared;
using Avalonia.WebView.Linux.Shared.Interoperates;
using Gio.Internal;

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

                var userContentManager = webView.UserContentManager;

                var script = GtkApi.CreateUserScriptX(BlazorScriptHelper.BlazorStartingScript);
                GtkApi.AddScriptForUserContentManager(userContentManager.Handle, script);
                GtkApi.ReleaseScript(script);

                GtkApi.AddSignalConnect(
                    userContentManager.Handle,
                    $"script-message-received::{_messageKeyWord}",
                    LinuxApplicationManager.LoadFunction(_userContentMessageReceived),
                    IntPtr.Zero
                );
                GtkApi.RegisterScriptMessageHandler(userContentManager.Handle, _messageKeyWord);
            })
            .Result;

        _isBlazorWebView = true;
        return Task.CompletedTask;
    }

    void WebView_WebMessageReceived(nint pContentManager, nint pJsResult, nint pArg)
    {
        //var userContentManager = new UserContentManager(pContentManager);
        //var jsValue = JavascriptResult.New(pJsResult);

        if (_provider is null)
            return;

        var pJsStringValue = GtkApi.CreateJavaScriptResult(pJsResult);
        if (!pJsStringValue.IsStringEx())
            return;

        var message = new WebViewMessageReceivedEventArgs
        {
            Message = pJsStringValue.ToStringEx(),
            Source = _provider.BaseUri,
        };
        GtkApi.ReleaseJavaScriptResult(pJsResult);

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
        nint streamPtr = MemoryInputStream.NewFromData(ref ms.GetBuffer()[0], (uint)ms.Length, _ => { });
        using var inputStream = new InputStream(streamPtr, false);
        response.Content.CopyTo(ms);

        var pBuffer = GtkApi.MarshalToGLibInputStream(ms.GetBuffer(), ms.Length);
        request.Finish(inputStream, ms.Length);
    }
}
