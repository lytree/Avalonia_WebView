﻿namespace Avalonia.WebViews.Core.Shared;

public static class IAvaloniaHandlerCollectionExtensions
{
    public static IAvaloniaHandlerCollection AddHandler<TType, TTypeRender>(
        this IAvaloniaHandlerCollection handlersCollection
    )
    {
        return handlersCollection;
    }
}
