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
        uniform sampler2D uCells;
        uniform ivec2 uGrid;
        uniform vec2 uFramebuffer;
        out vec4 fragColor;
        void main()
        {
            vec2 uv = gl_FragCoord.xy / uFramebuffer;
            ivec2 cell = ivec2(uv.x * float(uGrid.x), (1.0 - uv.y) * float(uGrid.y));
            cell = clamp(cell, ivec2(0), uGrid - 1);
            fragColor = vec4(texelFetch(uCells, cell, 0).rgb, 1.0);
        }
        """;

    public Int2 Size { get; }

    private readonly Color[] _cells;
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
        _cells = new Color[size.X * size.Y];

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
        _gl.TexImage2D<Color>(TextureTarget.Texture2D, 0, InternalFormat.Rgba8,
            (uint)size.X, (uint)size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _cells);

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

                Array.Clear(_cells);
                Context context = new(frame, elapsed, delta, Size, _cells);
                painter.Paint(context);

                _gl.BindTexture(TextureTarget.Texture2D, _texture);
                _gl.TexSubImage2D<Color>(TextureTarget.Texture2D, 0, 0, 0,
                    (uint)Size.X, (uint)Size.Y, PixelFormat.Rgba, PixelType.UnsignedByte, _cells);

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