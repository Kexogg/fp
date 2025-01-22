using TagsCloudContainerCore.Models.Graphics;

namespace TagsCloudContainerCore.FontManager;

public record FontManagerConfig
{
    public Font Font { get; init; } = new("Arial");
}