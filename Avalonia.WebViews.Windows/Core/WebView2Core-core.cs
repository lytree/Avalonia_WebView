﻿using Avalonia.WebViews.Core.Enums;
using Avalonia.WebViews.Core.Events;
using Avalonia.WebViews.Core.Helpers;
using Avalonia.WebViews.Core.Models;

namespace Avalonia.WebViews.Windows.Core;

sealed partial class WebView2Core
{
    private Task<CoreWebView2Environment> CreateEnvironmentAsync()
    {
        var options = new CoreWebView2EnvironmentOptions(
            _creationProperties.AdditionalBrowserArguments!,
            _creationProperties.Language!
        );
        //return CoreWebView2Environment.CreateAsync(_creationProperties.BrowserExecutableFolder!, _creationProperties.UserDataFolder!, options);
        return CoreWebView2Environment.CreateAsync(
            _creationProperties.BrowserExecutableFolder!,
            _creationProperties.UserDataFolder!,
            options
        );
    }

    private CoreWebView2ControllerOptions? CreateCoreWebView2ControllerOptions(
        CoreWebView2Environment environment
    )
    {
        if (
            string.IsNullOrWhiteSpace(_creationProperties.ProfileName)
            && _creationProperties.IsInPrivateModeEnabled is null
        )
            return default;

        var coreWebView2ControllerOptions = environment.CreateCoreWebView2ControllerOptions();
        coreWebView2ControllerOptions.ProfileName = _creationProperties.ProfileName!;
        coreWebView2ControllerOptions.IsInPrivateModeEnabled =
            _creationProperties.IsInPrivateModeEnabled.GetValueOrDefault();

        return coreWebView2ControllerOptions;
    }

    private void SetEnvironmentDefaultBackground(Color color) =>
        Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", color.Name);

    private async void CoreWebView2_WebResourceRequested(
        object sender,
        CoreWebView2WebResourceRequestedEventArgs e
    )
    {
        await CoreWebView2_WebResourceRequestedAsync(sender, e);
    }

    private string GetHeaderString(IDictionary<string, string> headers) =>
        string.Join(Environment.NewLine, headers.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

    private Task CoreWebView2_WebResourceRequestedAsync(
        object _,
        CoreWebView2WebResourceRequestedEventArgs e
    )
    {
        if (_provider is null)
            return Task.CompletedTask;

        if (_coreWebView2Environment is null)
            return Task.CompletedTask;

        var allowFallbackOnHostPage =
            e.ResourceContext == CoreWebView2WebResourceContext.Document
            || e.ResourceContext == CoreWebView2WebResourceContext.Other;

        var request = new WebResourceRequest
        {
            RequestUri = e.Request.Uri,
            AllowFallbackOnHostPage = allowFallbackOnHostPage
        };

        if (!_provider.PlatformWebViewResourceRequested(this, request, out var response))
            return Task.CompletedTask;

        if (response is null)
            return Task.CompletedTask;

        var headerString = GetHeaderString(response.Headers);
        e.Response = _coreWebView2Environment.CreateWebResourceResponse(
            response.Content,
            response.StatusCode,
            response.StatusMessage,
            headerString
        );
        return Task.CompletedTask;
    }

    private void CoreWebView2Controller_ZoomFactorChanged(object sender, object e) { }

    private void CoreWebView2Controller_MoveFocusRequested(
        object sender,
        CoreWebView2MoveFocusRequestedEventArgs e
    )
    { }

    private void CoreWebView2Controller_LostFocus(object sender, object e) { }

    private void CoreWebView2Controller_GotFocus(object sender, object e) { }

    private void CoreWebView2Controller_AcceleratorKeyPressed(
        object? sender,
        CoreWebView2AcceleratorKeyPressedEventArgs e
    )
    { }

    private void CoreWebView2_DOMContentLoaded(
        object? sender,
        CoreWebView2DOMContentLoadedEventArgs e
    )
    { }

    private void CoreWebView2_WebMessageReceived(
        object? sender,
        CoreWebView2WebMessageReceivedEventArgs e
    )
    {
        var message = new WebViewMessageReceivedEventArgs
        {
            Message = e.TryGetWebMessageAsString(),
            MessageAsJson = e.WebMessageAsJson,
            Source = new Uri(e.Source),
            RawArgs = e,
        };
        _callBack.PlatformWebViewMessageReceived(this, message);
        _provider?.PlatformWebViewMessageReceived(this, message);
    }

    private void CoreWebView2_SourceChanged(
        object? sender,
        CoreWebView2SourceChangedEventArgs e
    )
    { }

    private void CoreWebView2_ProcessFailed(
        object? sender,
        CoreWebView2ProcessFailedEventArgs e
    )
    { }

    private void CoreWebView2_NavigationStarting(
        object sender,
        CoreWebView2NavigationStartingEventArgs e
    )
    {
        WebViewUrlLoadingEventArg args = new() { Url = new Uri(e.Uri), RawArgs = e };
        _callBack.PlatformWebViewNavigationStarting(this, args);
        e.Cancel = args.Cancel;
    }

    private void CoreWebView2_NavigationCompleted(
        object sender,
        CoreWebView2NavigationCompletedEventArgs e
    )
    {
        _callBack.PlatformWebViewNavigationCompleted(
            this,
            new WebViewUrlLoadedEventArg() { IsSuccess = e.IsSuccess, RawArgs = e }
        );
    }

    private void CoreWebView2_HistoryChanged(object sender, object e) { }

    private void CoreWebView2_ContentLoading(
        object sender,
        CoreWebView2ContentLoadingEventArgs e
    )
    { }

    private void CoreWebView2_NewWindowRequested(
        object sender,
        CoreWebView2NewWindowRequestedEventArgs e
    )
    {
        var urlLoadingStrategy = UrlRequestStrategy.OpenInWebView;
        var uri = new Uri(e.Uri);

        if (_provider is not null)
        {
            if (_provider.BaseUri.IsBaseOf(uri))
                urlLoadingStrategy = UrlRequestStrategy.OpenInWebView;
        }

        var newWindowEventArgs = new WebViewNewWindowEventArgs()
        {
            Url = uri,
            UrlLoadingStrategy = urlLoadingStrategy,
            RawArgs = e,
        };

        if (!_callBack.PlatformWebViewNewWindowRequest(this, newWindowEventArgs))
            return;

        switch (newWindowEventArgs.UrlLoadingStrategy)
        {
            case UrlRequestStrategy.OpenExternally:
                e.Handled = true;
                OpenUriHelper.OpenInProcess(uri);
                break;
            case UrlRequestStrategy.OpenInWebView:
                e.NewWindow = CoreWebView2!;
                break;
            case UrlRequestStrategy.CancelLoad:
                e.Handled = true;
                break;
            case UrlRequestStrategy.OpenInNewWindow:
            default:
                break;
        }
    }

    private void Profile_Deleted(object? sender, object e) { }
}
