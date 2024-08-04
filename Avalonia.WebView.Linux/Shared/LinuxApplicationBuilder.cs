using Avalonia.WebView.Linux.Shared.Core;

namespace Avalonia.WebView.Linux.Shared;

public class LinuxApplicationBuilder
{
    public static ILinuxApplication Build(bool isWslDevelop) => new LinuxApplication(isWslDevelop);
}
