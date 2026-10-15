using System.Diagnostics;
using Etch.Common;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Etch.Backend.OpenGL;

public sealed class Surface : ISurface, IDisposable
{
    public const int CellWidth = 8;
    public const int CellHeight = 16;

    private const string VertexSource = """
        #version 330 core
        void main()
        {
            vec2 p = vec2((gl_VertexID << 1) & 2, gl_VertexID & 2);
            gl_Position = vec4(p * 2.0 - 1.0, 0.0, 1.0);
        }
        """;

    private const string FragmentSource = """
        #version 330 core
        uniform usampler2D uCells;
        uniform ivec2 uGrid;
        uniform vec2 uFramebuffer;
        out vec4 fragColor;
        void main()
        {
            vec2 uv = gl_FragCoord.xy / uFramebuffer;
            ivec2 cell = ivec2(uv.x * float(uGrid.x), (1.0 - uv.y) * float(uGrid.y));
            cell = clamp(cell, ivec2(0), uGrid - 1);

            // .r = low word of the Etchel (glyph + foreground), .g = high word (background in bits 0-23).
            uint background = texelFetch(uCells, cell, 0).g;
            fragColor = vec4(
                float(background & 0xFFu),
                float((background >> 8) & 0xFFu),
                float((background >> 16) & 0xFFu),
                255.0) / 255.0;
        }
        """;

    public Int2 Size { get; }

    private readonly Etchel[] _etchels;
    private readonly IWindow _window;
    private readonly GL _gl;
    private readonly uint _texture;
    private readonly uint _vao;
    private readonly uint _program;
    private readonly int _gridLocation;
    private readonly int _framebufferLocation;
    private bool _disposed;

    public static Surface Default => new(new Int2(80, 40));

    public Surface(Int2 size)
    {
        Size = size;
        _etchels = new Etchel[size.X * size.Y];

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
        _gl = _window.CreateOpenGL();

        Vector2D<int> framebuffer = _window.FramebufferSize;
        _gl.Viewport(0, 0, (uint)framebuffer.X, (uint)framebuffer.Y);

        _texture = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
        _gl.TexImage2D<Etchel>(TextureTarget.Texture2D, 0, InternalFormat.RG32ui,
            (uint)size.X, (uint)size.Y, 0, PixelFormat.RGInteger, PixelType.UnsignedInt, _etchels);

        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);

        _program = BuildProgram(_gl, VertexSource, FragmentSource);
        _gridLocation = _gl.GetUniformLocation(_program, "uGrid");
        _framebufferLocation = _gl.GetUniformLocation(_program, "uFramebuffer");

        _window.FramebufferResize += OnFramebufferResize;
    }

    public void Run<TPainter>(TPainter painter, int until = 0) where TPainter : IPainter, allows ref struct
    {
        try
        {
            long frame = 0;
            long start = Stopwatch.GetTimestamp();
            long last = start;

            while (!_window.IsClosing)
            {
                _window.DoEvents();
                if (_window.IsClosing) break;

                long now = Stopwatch.GetTimestamp();
                float delta = (float)Stopwatch.GetElapsedTime(last, now).TotalSeconds;
                double elapsed = Stopwatch.GetElapsedTime(start, now).TotalSeconds;
                last = now;

                Array.Clear(_etchels);
                Context context = new(frame, elapsed, delta, Size, _etchels);
                painter.Paint(context);

                _gl.BindTexture(TextureTarget.Texture2D, _texture);
                _gl.TexSubImage2D<Etchel>(TextureTarget.Texture2D, 0, 0, 0,
                    (uint)Size.X, (uint)Size.Y, PixelFormat.RGInteger, PixelType.UnsignedInt, _etchels);

                Draw(_window.FramebufferSize);
                _window.SwapBuffers();
                frame++;

                if (until > 0 && frame >= until) break;
            }
        }
        finally
        {
            Dispose();
        }
    }

    public void Stop() => _window.Close();

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _window.FramebufferResize -= OnFramebufferResize;
        _gl.DeleteTexture(_texture);
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteProgram(_program);
        _gl.Dispose();
        _window.Dispose();
    }

    private void OnFramebufferResize(Vector2D<int> size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
        Draw(size);
        _window.SwapBuffers();
    }

    private void Draw(Vector2D<int> framebuffer)
    {
        _gl.UseProgram(_program);
        _gl.BindVertexArray(_vao);
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        _gl.Uniform2(_gridLocation, Size.X, Size.Y);
        _gl.Uniform2(_framebufferLocation, (float)framebuffer.X, (float)framebuffer.Y);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }

    private static uint BuildProgram(GL gl, string vertexSource, string fragmentSource)
    {
        uint vertex = Compile(gl, ShaderType.VertexShader, vertexSource);
        uint fragment = Compile(gl, ShaderType.FragmentShader, fragmentSource);

        uint program = gl.CreateProgram();
        gl.AttachShader(program, vertex);
        gl.AttachShader(program, fragment);
        gl.LinkProgram(program);
        gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int status);

        gl.DetachShader(program, vertex);
        gl.DetachShader(program, fragment);
        gl.DeleteShader(vertex);
        gl.DeleteShader(fragment);

        if (status == 0)
        {
            string log = gl.GetProgramInfoLog(program);
            gl.DeleteProgram(program);
            throw new InvalidOperationException($"Shader program failed to link: {log}");
        }
        return program;
    }

    private static uint Compile(GL gl, ShaderType type, string source)
    {
        uint shader = gl.CreateShader(type);
        gl.ShaderSource(shader, source);
        gl.CompileShader(shader);
        gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
        if (status == 0)
        {
            string log = gl.GetShaderInfoLog(shader);
            gl.DeleteShader(shader);
            throw new InvalidOperationException($"{type} failed to compile: {log}");
        }
        return shader;
    }
}