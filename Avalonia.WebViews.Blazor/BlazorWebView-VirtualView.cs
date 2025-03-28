﻿namespace Avalonia.WebViews.Blazor;

partial class BlazorWebView
{
    BlazorWebView IVirtualWebView<BlazorWebView>.VirtualView => this;

    object IVirtualWebView.VirtualViewObject => this;

    IPlatformWebView? IVirtualWebView.PlatformView => PlatformWebView;
}
