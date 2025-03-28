using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.WebViews.Core;
using Avalonia.WebViews.Core.Configurations;
using Avalonia.WebViews.Core.Extensions;
using Avalonia.WebViews.Core.Shared;
using Avalonia.WebViews.Linux.Shared;

namespace Avalonia.WebViews.Linux;

internal class ViewHandlerProvider : IViewHandlerProvider
{
    public ViewHandlerProvider()
    {
        _linuxApplication = GlobalVariables.LinuxApplication;
        var bRet = _linuxApplication.RunAsync(default, default).Result;
        if (!bRet)
            throw new ArgumentNullException(
                nameof(ILinuxApplication),
                "create gtk application failed!"
            );

        if (
            Application.Current?.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime deskTop
        )
        {
            deskTop.ShutdownRequested += DeskTop_ShutdownRequested;
        }

    }

    readonly ILinuxApplication _linuxApplication;

    IViewHandlerProvider IViewHandlerProvider.ViewHandlerProvider => this;

    IViewHandler IViewHandlerProvider.CreatePlatformWebViewHandler(
        IVirtualWebView virtualView,
        IVirtualWebViewControlCallBack virtualViewCallBack,
        IVirtualBlazorWebViewProvider? provider,
        Action<WebViewCreationProperties>? configDelegate
    )
    {
        var creatonProperty = new WebViewCreationProperties();
        configDelegate?.Invoke(creatonProperty);

        return new WebViewHandler(
            _linuxApplication,
            virtualView,
            virtualViewCallBack,
            provider,
            creatonProperty
        );
    }

    private void DeskTop_ShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        _linuxApplication.Dispose();
    }
}
