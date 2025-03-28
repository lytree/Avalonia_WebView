#nullable disable

namespace Avalonia.WebViews.Core.Shared.Handlers;

public abstract class ViewHandler<TVirtualViewContext, TPlatformViewContext> : ViewHandler
    where TVirtualViewContext : class
    where TPlatformViewContext : class
{
    protected ViewHandler()
    {
        HandleDescriptor = typeof(TPlatformViewContext).FullName;
    }

    public TPlatformViewContext PlatformViewContext
    {
        get => PlatformViewContextObject as TPlatformViewContext;
        protected set => PlatformViewContextObject = value;
    }

    public TVirtualViewContext VirtualViewContext
    {
        get => VisualViewContextObject as TVirtualViewContext;
        protected set => VisualViewContextObject = value;
    }
}
