using Etch.Backend;
using Etch.Backend.OpenGL;
using Etch.Painters;

Etch.Diagnostics.Health.Check();
Renderer.Start(Surface.Default(), new Spiral(), TimeSpan.FromSeconds(1.0 / 15));