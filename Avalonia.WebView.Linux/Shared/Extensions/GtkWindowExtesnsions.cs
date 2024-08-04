using Avalonia.WebView.Linux.Shared.Interoperates;

namespace Avalonia.WebView.Linux.Shared.Extensions;

public static class GtkWindowExtesnsions
{
    public static nint X11Handle(this GWidget widget) => GtkApi.GetWidgetXid(widget);
}
