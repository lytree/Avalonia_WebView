using Avalonia.WebViews.Linux.Shared.Core;

namespace Avalonia.WebViews.Linux.Shared;

public class LinuxApplicationBuilder
{
    public static ILinuxApplication Build(bool isWslDevelop) => new LinuxApplication(isWslDevelop);
}
