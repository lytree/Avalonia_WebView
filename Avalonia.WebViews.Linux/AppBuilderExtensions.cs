using Avalonia.WebViews.Core.Configurations;
using Avalonia.WebViews.Core.Extensions;
using Avalonia.WebViews.Core.Shared;
using Avalonia.WebViews.Linux.Shared;
using Avalonia.WebViews.Linux.Shared.Core;
using Avalonia.WebViews;
namespace Avalonia.WebViews.Linux;

public static class AppBuilderExtensions
{
    public static AppBuilder UseLinuxWebView(this AppBuilder builder, Func<WebViewCreationProperties>? config, bool isWslDevelop)
    {
        GlobalVariables.LinuxApplication = new LinuxApplication(isWslDevelop);
        WebView.ViewHandlerProvider = new ViewHandlerProvider(config);
        return builder;
    }
}
