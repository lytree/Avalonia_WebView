﻿namespace Avalonia.WebViews.Core;

public interface IPlatformWebView<T> : IPlatformWebView
{
    T PlatformView { get; }
}
