﻿namespace Avalonia.WebViews.Blazor.Configurations;
public class JsComponentConfigration : IJSComponentConfiguration
{
    public JsComponentConfigration(JSComponentConfigurationStore jSComponentConfigurationStore)
    {
        JSComponents = jSComponentConfigurationStore;
    }

    public JSComponentConfigurationStore JSComponents { get; init; }
}
