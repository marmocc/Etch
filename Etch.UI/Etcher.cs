using System.Collections.Concurrent;
using Etch.Platform;
using Etch.Deferred;

namespace Etch.UI;

public sealed class Etcher<TContext, TWindow> : IDisposable
    where TContext : IContext, allows ref struct 
    where TWindow : IWindow<TContext>
{
    private readonly Deferrer _deferrer = new();
    private readonly List<IWidget> _widgets = new();
    private readonly ConcurrentQueue<Action> _posted = new();
    private readonly TWindow _window;
    private readonly Thread _thread;
    private bool _disposed;

    internal Etcher(TWindow window)
    {
        _window = window;
        _window.Rendering += OnRendering;
        _thread = new(_window.Run) { IsBackground = false, Name = "Etch UI Thread" };
    }

    public List<IWidget> Widgets => _widgets;

    public static Etcher<TContext, TWindow> Create(TWindow window) => new(window);
    public Etcher<TContext, TWindow> Widget(IWidget widget)
    {
        _widgets.Add(widget);
        widget.Invalidated += OnWidgetInvalidated;
        return this;
    }

    public void Post(Action mutation)
    {
        _posted.Enqueue(mutation);
        _window.Invalidate();
    }

    public void Run() => _thread?.Start();

    private void OnRendering(TContext context)
    {
        while (_posted.TryDequeue(out var action))
            action();

        _deferrer.Flush();

        foreach (var widget in _widgets.OrderBy(w => w.ZIndex.Current))
            widget.Render(context);
    }

    private void OnWidgetInvalidated() => _window.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _window.Close();
        _thread?.Join();
        _window.Dispose();
    }
}