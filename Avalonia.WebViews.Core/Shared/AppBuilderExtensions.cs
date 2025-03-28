namespace Avalonia.WebViews.Core.Shared;

public static class AppBuilderExtensions
{
    public static AppBuilder ConfigureAvaloniaHandlers(
        this AppBuilder builder,
        Action<IAvaloniaHandlerCollection> configureDelegate
    )
    {
        AvaloniaHandlerCollection list = new();
        configureDelegate?.Invoke(list);
        return builder;
    }
}
