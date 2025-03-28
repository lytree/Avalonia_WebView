using Avalonia.WebView.Core;
using Avalonia.WebView.Core.Configurations;
using Avalonia.WebView.Core.Extensions;
using Avalonia.WebView.Core.Shared;
using Avalonia.WebView.Helpers;

namespace Avalonia.WebView;

public sealed partial class WebView
    : Control,
        IVirtualWebView<WebView>,
        IEmptyView,
        IWebViewEventHandler,
        IVirtualWebViewControlCallBack,
        IWebViewControl
{
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

    public WebView(WebViewCreationProperties properties, IViewHandlerProvider viewHandlerProvider)
    {
        _viewHandlerProvider = viewHandlerProvider;
        _creationProperties = properties;
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

    private readonly WebViewCreationProperties _creationProperties;
    private readonly BorderRenderHelper _borderRenderHelper = new();
    private readonly IViewHandlerProvider _viewHandlerProvider;

    private readonly Border _partInnerContainer;

    private double _scale;
    private Thickness? _layoutThickness;

    public IPlatformWebView? PlatformWebView { get; private set; }
}
