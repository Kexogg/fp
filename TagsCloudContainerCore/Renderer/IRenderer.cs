using SkiaSharp;
using TagsCloudContainerCore.Models;

namespace TagsCloudContainerCore.Renderer;

public interface IRenderer<TConfig> : IRenderer
{
    TConfig Config { get; init; }
}

public interface IRenderer
{
    Result<SKImage> DrawTags(IReadOnlyCollection<Tag> tags);
}