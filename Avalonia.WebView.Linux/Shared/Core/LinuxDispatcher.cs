namespace Avalonia.WebView.Linux.Shared.Core;

internal class LinuxDispatcher : ILinuxDispatcher
{
    private bool _isRunning;

    bool ILinuxDispatcher.Start()
    {
        _isRunning = true;
        return true;
    }

    bool ILinuxDispatcher.Stop()
    {
        _isRunning = false;
        return true;
    }

    Task<bool> ILinuxDispatcher.InvokeAsync(Action action)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

        if (!_isRunning)
            return Task.FromResult(false);

        return RunOnGlibThread(() =>
        {
            var task = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                GApplication.Invoke(
                    (_, _) =>
                    {
                        action();
                        task.SetResult(true);
                    }
                );
            });
            return task.Task;
        }).Result;
    }

    Task<bool> ILinuxDispatcher.InvokeAsync(Action<object?, EventArgs> action)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

        if (!_isRunning)
            return Task.FromResult(false);

        return RunOnGlibThread(() =>
        {
            var task = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                GApplication.Invoke(
                    (s, e) =>
                    {
                        action(s, e);
                        task.SetResult(true);
                    }
                );
            });
            return task.Task;
        }).Result;
    }

    Task<bool> ILinuxDispatcher.InvokeAsync(
        object? sender,
        EventArgs args,
        Action<object?, EventArgs> action
    )
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

        if (!_isRunning)
            return Task.FromResult(false);

        return RunOnGlibThread(() =>
        {
            var task = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                GApplication.Invoke(
                    sender,
                    args,
                    (s, e) =>
                    {
                        action(s, e);
                        task.SetResult(true);
                    }
                );
            });
            return task.Task;
        }).Result;
    }

    Task<T> ILinuxDispatcher.InvokeAsync<T>(Func<T> func)
    {
        if (func is null)
            throw new ArgumentNullException(nameof(func));

        if (!_isRunning)
            return Task.FromResult<T>(default!);

        return RunOnGlibThread(() =>
        {
            var task = new TaskCompletionSource<T>();
            Task.Run(() =>
            {
                GApplication.Invoke(
                    (_, _) =>
                    {
                        var ret = func.Invoke();
                        task.SetResult(ret);
                    }
                );
            });
            return task.Task;
        }).Result;
    }

    Task<T> ILinuxDispatcher.InvokeAsync<T>(Func<object?, EventArgs, T> func)
    {
        if (func is null)
            throw new ArgumentNullException(nameof(func));

        if (!_isRunning)
            return Task.FromResult<T>(default!);

        return RunOnGlibThread(() =>
        {
            var task = new TaskCompletionSource<T>();
            Task.Run(() =>
            {
                GApplication.Invoke(
                    (s, e) =>
                    {
                        var ret = func.Invoke(s, e);
                        task.SetResult(ret);
                    }
                );
            });
            return task.Task;
        }).Result;
    }

    Task<T> ILinuxDispatcher.InvokeAsync<T>(
        object? sender,
        EventArgs args,
        Func<object?, EventArgs, T> func
    )
    {
        if (func is null)
            throw new ArgumentNullException(nameof(func));

        if (!_isRunning)
            return Task.FromResult<T>(default!);

        return RunOnGlibThread(() =>
        {
            var task = new TaskCompletionSource<T>();
            Task.Run(() =>
            {
                GApplication.Invoke(
                    sender,
                    args,
                    (s, e) =>
                    {
                        var ret = func.Invoke(s, e);
                        task.SetResult(ret);
                    }
                );
            });
            return task.Task;
        }).Result;
    }
}
