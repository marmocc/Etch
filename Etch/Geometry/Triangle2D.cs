using System.Numerics;

namespace Etch.Geometry;

public readonly struct Triangle2D<T>(Vector2<T> a, Vector2<T> b, Vector2<T> c) where T : INumber<T>
{
    public readonly Vector2<T> A = a, B = b, C = c;
    public Rectangle2D<T> Bounds => GetBounds();

    public bool Contains(Vector2<T> point)
    {
        // Barycentric technique
        var v0 = C - A;
        var v1 = B - A;
        var v2 = point - A;

        var dot00 = v0.X * v0.X + v0.Y * v0.Y;
        var dot01 = v0.X * v1.X + v0.Y * v1.Y;
        var dot02 = v0.X * v2.X + v0.Y * v2.Y;
        var dot11 = v1.X * v1.X + v1.Y * v1.Y;
        var dot12 = v1.X * v2.X + v1.Y * v2.Y;

        var denom = (dot00 * dot11 - dot01 * dot01);
        if (denom == T.Zero) return false;

        var u = (dot11 * dot02 - dot01 * dot12);
        var v = (dot00 * dot12 - dot01 * dot02);

        if (denom < T.Zero)
            return u <= T.Zero && v <= T.Zero && (u + v) > denom;
        return u >= T.Zero && v >= T.Zero && (u + v) < denom;
    }

    private Rectangle2D<T> GetBounds()
    {
        var minX = T.Min(A.X, T.Min(B.X, C.X));
        var minY = T.Min(A.Y, T.Min(B.Y, C.Y));
        var maxX = T.Max(A.X, T.Max(B.X, C.X));
        var maxY = T.Max(A.Y, T.Max(B.Y, C.Y));
        return new(new Vector2<T>(minX, minY), 
                   new Vector2<T>(maxX - minX, maxY - minY));
    }
}