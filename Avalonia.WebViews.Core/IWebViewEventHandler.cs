using Avalonia.WebViews.Core.Events;

namespace Avalonia.WebViews.Core;

public interface IWebViewEventHandler
{
    event EventHandler<WebViewCreatingEventArgs>? WebViewCreating;
    event EventHandler<WebViewCreatedEventArgs>? WebViewCreated;

    event EventHandler<WebViewUrlLoadingEventArg>? NavigationStarting;
    event EventHandler<WebViewUrlLoadedEventArg>? NavigationCompleted;

    event EventHandler<WebViewMessageReceivedEventArgs>? WebMessageReceived;
    event EventHandler<WebViewNewWindowEventArgs>? WebViewNewWindowRequested;
}
