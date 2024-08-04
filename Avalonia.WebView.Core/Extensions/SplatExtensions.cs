using Splat;

namespace Avalonia.WebView.Core.Extensions;

public static class SplatExtensions
{
    public static T GetRequiredService<T>(
        this IReadonlyDependencyResolver resolver,
        string? contract = null
    )
    {
        var service = (T?)resolver.GetService(typeof(T), contract);
        ArgumentNullException.ThrowIfNull(service);
        return service;
    }

    public static void RegisterLazySingleton<TService, TImplementation>(
        this IMutableDependencyResolver resolver,
        string? contract = null
    )
        where TImplementation : TService, new() =>
        resolver.RegisterLazySingleton<TService>(() => new TImplementation(), contract);
}
