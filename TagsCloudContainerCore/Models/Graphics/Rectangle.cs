namespace TagsCloudContainerCore.Models.Graphics;

public readonly struct Rectangle
{
    public Rectangle(float left, float top, float right, float bottom)
    {
        if (left > right || top > bottom)
            throw new ArgumentException("Invalid rectangle parameters");
        if (float.IsNaN(left) || float.IsNaN(top) || float.IsNaN(right) || float.IsNaN(bottom))
            throw new ArgumentException("Rectangle parameters must not be NaN");
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public float Left { get; }
    public float Top { get; }
    public float Right { get; }
    public float Bottom { get; }

    public float Width => Right - Left;
    public float Height => Bottom - Top;

    public bool IntersectsWith(Rectangle other)
    {
        return Left < other.Right &&
               Right > other.Left &&
               Top < other.Bottom &&
               Bottom > other.Top;
    }
}