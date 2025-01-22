namespace TagsCloudContainerCore.Models.Graphics;

public struct Size(float width, float height)
{
    public float Width { get; } = width;
    public float Height { get; } = height;
}