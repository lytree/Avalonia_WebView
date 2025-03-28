namespace Avalonia.WebViews;

partial class WebView
{
    private async Task<bool> Navigate(Uri? uri)
    {
        if (uri is null)
            return false;

        if (PlatformWebView is null)
            return false;

        if (!PlatformWebView.IsInitialized)
        {
            var bRet = await PlatformWebView.Initialize();
            if (!bRet)
                return false;
        }

        return PlatformWebView.Navigate(uri);
    }

    private async Task<bool> NavigateToString(string? htmlContent)
    {
        if (string.IsNullOrWhiteSpace(htmlContent))
            return false;

        if (PlatformWebView is null)
            return false;

        if (!PlatformWebView.IsInitialized)
        {
            var bRet = await PlatformWebView.Initialize();
            if (!bRet)
                return false;
        }

        return PlatformWebView.NavigateToString(htmlContent!);
    }
}
