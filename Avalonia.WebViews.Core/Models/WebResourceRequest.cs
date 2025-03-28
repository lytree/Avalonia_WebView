﻿namespace Avalonia.WebViews.Core.Models;

public class WebResourceRequest
{
    public required string RequestUri { get; set; }

    public required bool AllowFallbackOnHostPage { get; set; }
}
