using Avalonia.WebViews.Core;
using Avalonia.WebViews.Core.Configurations;
using Avalonia.WebViews.Core.Extensions;
using Avalonia.WebViews.Core.Shared;
using Avalonia.WebViews.Helpers;

namespace Avalonia.WebViews;

public sealed partial class WebView
    : Control,
        IVirtualWebView<WebView>,
        IEmptyView,
        IWebViewEventHandler,
        IVirtualWebViewControlCallBack,
        IWebViewControl
{
    public static IViewHandlerProvider? ViewHandlerProvider { get; set; } = default;
    public static WebViewCreationProperties WebViewCreationProperties { get; set; } = new();
    static WebView()
    {
        AffectsRender<WebView>(
            BackgroundProperty,
            BorderBrushProperty,
            BorderThicknessProperty,
            CornerRadiusProperty,
            BoxShadowProperty
        );
        AffectsMeasure<WebView>(ChildProperty, PaddingProperty, BorderThicknessProperty);
        LoadDependencyObjectsChanged();
        LoadHostDependencyObjectsChanged();
    }

    public WebView()
    {
        ClipToBounds = false;

        ContentPresenter partEmptyViewPresenter =
            new()
            {
                [!ContentPresenter.ContentProperty] = this[!EmptyViewerProperty],
                [!ContentPresenter.ContentTemplateProperty] = this[!EmptyViewerTemplateProperty],
            };

        _partInnerContainer = new()
        {
            Child = partEmptyViewPresenter,
            ClipToBounds = true,
            [!Border.CornerRadiusProperty] = this[!CornerRadiusProperty]
        };
        Child = _partInnerContainer;
    }
    private readonly BorderRenderHelper _borderRenderHelper = new();

    private readonly Border _partInnerContainer;

    private double _scale;
    private Thickness? _layoutThickness;

    public IPlatformWebView? PlatformWebView { get; private set; }



}
