namespace Avalonia.WebView.Blazor;

internal class BlazorWebViewBuild 
{
    public BlazorWebViewBuild(IServiceCollection _serviceCollection, Action<BlazorWebViewSetting> configDelegate)
    {
        _serviceCollection.AddOptions<BlazorWebViewSetting>().Configure(configDelegate);
        _serviceCollection.AddBlazorWebView()
                          .AddSingleton<JSComponentConfigurationStore>()
                          .AddSingleton(provider => new AvaloniaDispatcher(AvaloniaUIDispatcher.UIThread))
                          .AddSingleton<IJSComponentConfiguration>(provider =>new JsComponentConfigration(provider.GetRequiredService<JSComponentConfigurationStore>()));
    }

    readonly IServiceCollection _serviceCollection;
    public IServiceProvider ServiceProvider => _serviceCollection.BuildServiceProvider();
}
