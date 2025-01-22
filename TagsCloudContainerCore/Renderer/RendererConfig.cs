using TagsCloudContainerCore.Models.Graphics;

namespace TagsCloudContainerCore.Renderer;

public record RendererConfig()
{
    private readonly float _renderingScale = 1;

    public Color BackgroundColor { get; init; } = new(255, 255, 255);
    public Color TextColor { get; init; } = new(0, 0, 0);
    public Font TagFont { get; init; } = new("Arial", 12);
    public float RenderingScale
    {
        get => _renderingScale;
        init
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(RenderingScale),
                    "Rendering scale must be greater than 0.");
            _renderingScale = value;
        }
    }
}