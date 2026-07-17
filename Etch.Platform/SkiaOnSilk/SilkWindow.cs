using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using SkiaSharp;

namespace Etch.Platform.SkiaOnSilk;

public sealed class SilkWindow : IWindow<SkiaContext>
{
    private readonly IWindow _window;
    private GL? _gl;
    private GRContext? _grContext;
    private SKSurface? _surface;
    private bool _needsRender = true;

    public Float2 Size => new(_window.Size.X, _window.Size.Y);

    public event Action<Float2>? Resized;
    public event Renderer<SkiaContext>? Rendering;

    public SilkWindow(string title, int width, int height)
    {
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(width, height),
            Title = title,
            IsEventDriven = true,
            ShouldSwapAutomatically = false,
        };

        _window = Window.Create(options);
        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.FramebufferResize += OnResize;
    }

    private void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        var glInterface = GRGlInterface.Create(name => _gl.Context.TryGetProcAddress(name, out var addr) ? addr : 0);
        _grContext = GRContext.CreateGl(glInterface);
    }

    private void OnResize(Vector2D<int> framebufferSize)
    {
        _gl?.Viewport(framebufferSize);
        _surface?.Dispose();
        _surface = null;
        Resized?.Invoke(Size);
        Invalidate();
    }

    private void OnRender(double delta)
    {
        if (!_needsRender || _gl is null || _grContext is null) return;
        _needsRender = false;

        if (_surface is null)
        {
            var fbSize = _window.FramebufferSize;
            var fbInfo = new GRGlFramebufferInfo(0, 0x8058 /* GL_RGBA8 */);
            var target = new GRBackendRenderTarget(fbSize.X, fbSize.Y, 0, 8, fbInfo);
            _surface = SKSurface.Create(_grContext, target, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
        }

        Rendering?.Invoke(new SkiaContext(_surface.Canvas));
        _surface.Canvas.Flush();
        _window.SwapBuffers();
    }

    public void Invalidate()
    {
        if (_needsRender) return;
        _needsRender = true;
        _window.ContinueEvents();
    }

    public void Run() => _window.Run();
    public void Close() => _window.Close();

    public void Dispose()
    {
        _surface?.Dispose();
        _grContext?.Dispose();
        _window.Dispose();
    }
}