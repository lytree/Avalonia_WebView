namespace Avalonia.WebViews;

partial class WebView
{
    protected override Size MeasureOverride(Size availableSize)
    {
        return LayoutHelper.MeasureChild(Child, availableSize, Padding, BorderThickness);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return LayoutHelper.ArrangeChild(Child, finalSize, Padding, BorderThickness);
    }

    public override void Render(DrawingContext context)
    {
        _borderRenderHelper.Render(
            context,
            Bounds.Size,
            LayoutThickness,
            CornerRadius,
            Background,
            BorderBrush,
            BoxShadow
        );
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var viewHandler = ViewHandlerProvider.CreatePlatformWebViewHandler(
            this,
            this,
            default,
            config =>
            {
                config.AreDevToolEnabled = WebViewCreationProperties.AreDevToolEnabled;
                config.AreDefaultContextMenusEnabled =
                    WebViewCreationProperties.AreDefaultContextMenusEnabled;
                config.IsStatusBarEnabled = WebViewCreationProperties.IsStatusBarEnabled;
                config.BrowserExecutableFolder = WebViewCreationProperties.BrowserExecutableFolder;
                config.UserDataFolder = WebViewCreationProperties.UserDataFolder;
                config.Language = WebViewCreationProperties.Language;
                config.AdditionalBrowserArguments = WebViewCreationProperties.AdditionalBrowserArguments;
                config.ProfileName = WebViewCreationProperties.ProfileName;
                config.IsInPrivateModeEnabled = WebViewCreationProperties.IsInPrivateModeEnabled;
                config.DefaultWebViewBackgroundColor =
                    WebViewCreationProperties.DefaultWebViewBackgroundColor;
            }
        );

        if (viewHandler is null)
            throw new ArgumentNullException(nameof(viewHandler));

        var control = viewHandler.AttachableControl;
        if (control is null)
            return;
        //Child = control;
        _partInnerContainer.Child = control;
        PlatformWebView = viewHandler.PlatformWebView;

        await Navigate(Url);
        await NavigateToString(HtmlContent);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Child = null;
        PlatformWebView?.Dispose();
        PlatformWebView = null;
    }
}
