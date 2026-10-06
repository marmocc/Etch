using Etch.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Etch.Backend.OpenGL;

public sealed class Surface : ISurface<Context>, IDisposable
{
    public const int CellWidth = 8;
    public const int CellHeight = 16;

    private readonly IWindow _window;
    private readonly GL _gl;

    public static Surface Default => field ??= new(new Int2(80, 40));

    public Surface(Int2 size)
    {
        WindowOptions options = WindowOptions.Default with
        {
            API = new(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new(3, 3)),
            Size = new(CellWidth * size.X, CellHeight * size.Y),
            WindowBorder = WindowBorder.Resizable,
            VSync = false,
            Title = "Etch",
        };

        _window = Window.Create(options);
        _window.Initialize();
        _gl =_window.CreateOpenGL();
    }

    public void Dispose() { _gl.Dispose(); _window.Dispose(); }

    public void Run(Action<Context> draw)
    {
        while (!_window.IsClosing)
        {
            _window.DoEvents();
            _gl.ClearColor(1f, 0f, 0f, 1f);
            _gl.Clear(ClearBufferMask.ColorBufferBit);
            draw(new Context());
            _window.SwapBuffers();
        }
    }

    public void Stop() =>_window.Close();
}
