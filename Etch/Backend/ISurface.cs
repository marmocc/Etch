using Etch.Common;

namespace Etch.Backend;

public interface ISurface
{
    Int2 Size { get; }
    bool Stopping { get; }

    /// <summary>
    /// Presents the <c>Etchel</c>s on the <c>Surface</c>.
    /// </summary>
    void Present(ReadOnlySpan<Etchel> etchels);
    /// <summary>
    /// Blocks the Surface thread until an event happens or for a certain <paramref name="interval"/>.
    /// </summary>
    void Wait(TimeSpan interval);

    /// <summary>
    /// Wakes the Surface thread.
    /// <para><strong>This call is thread-safe.</strong></para>
    /// </summary>
    void Wake();
    /// <summary>
    /// Stops the Surface.
    /// <para><strong>This call is thread-safe.</strong></para>
    /// </summary>
    void Stop();
}