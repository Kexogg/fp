namespace TagsCloudContainerCore.Models.Graphics;

public struct Point
{
    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; }
    public float Y { get; }
}