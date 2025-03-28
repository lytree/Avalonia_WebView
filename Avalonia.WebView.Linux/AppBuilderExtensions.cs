using Avalonia.WebView.Core.Configurations;
using Avalonia.WebView.Core.Extensions;
using Avalonia.WebView.Core.Shared;
using Avalonia.WebView.Linux.Shared;
using Avalonia.WebView.Linux.Shared.Core;

namespace Avalonia.WebView.Linux;

public static class AppBuilderExtensions
{
    public static AppBuilder UseLinuxWebView(this AppBuilder builder, Func<WebViewCreationProperties>? config, bool isWslDevelop)
    {
        GlobalVariables.LinuxApplication = new LinuxApplication(isWslDevelop);
        return builder;
    }
}
