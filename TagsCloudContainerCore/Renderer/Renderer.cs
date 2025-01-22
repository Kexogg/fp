using Microsoft.Extensions.Logging;
using SkiaSharp;
using TagsCloudContainerCore.Models;
using TagsCloudContainerCore.Models.Graphics;

namespace TagsCloudContainerCore.Renderer;

public class Renderer : IRenderer<RendererConfig>
{
    private readonly ILogger<IRenderer> _logger;
    private readonly SKPaint _paint;
    public RendererConfig Config { get; init; } = new();


    public Renderer(ILogger<IRenderer> logger)
    {
        _logger = logger;
        _paint = new SKPaint();
    }

    public Result<SKImage> DrawTags(IReadOnlyCollection<Tag> tags)
    {
        if (tags.Count == 0)
        {
            return Result.Fail<SKImage>("No tags to draw");
        }
        _logger.LogInformation("Rendering {tags} tags with scale {scale}", tags.Count, Config.RenderingScale);
        var size = CalculateImageSize(tags).Value;
        var bitmap = new SKBitmap((int)(size.Width * Config.RenderingScale),
            (int)(size.Height * Config.RenderingScale));
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(new SKColor(Config.BackgroundColor.ToUint()));
        canvas.Translate(-size.Left * Config.RenderingScale, -size.Top * Config.RenderingScale);
        _paint.Color = new SKColor(Config.TextColor.ToUint());
        foreach (var tag in tags)
        {
            Config.TagFont.Size = tag.FontSize * Config.RenderingScale;
            var x = tag.BBox.Left * Config.RenderingScale;
            var y = tag.BBox.Bottom * Config.RenderingScale - Config.TagFont.Metrics.Descent;
            canvas.DrawText(tag.Text, x, y, Config.TagFont, _paint);
        }

        _logger.LogInformation("Finished drawing tags");
        return SKImage.FromBitmap(bitmap);
    }

    private Result<Rectangle> CalculateImageSize(IReadOnlyCollection<Tag> tags)
    {
        var endX = (int)tags.MaxBy(x => x.BBox.Right)!.BBox.Right;
        var endY = (int)tags.MaxBy(x => x.BBox.Bottom)!.BBox.Bottom;
        var startX = (int)tags.MinBy(x => x.BBox.Left)!.BBox.Left;
        var startY = (int)tags.MinBy(x => x.BBox.Top)!.BBox.Top;

        _logger.LogInformation("Calculated image size: {x}x{y}", endX - startX, endY - startY);
        return Result.Of(() => new Rectangle(startX, startY, endX, endY));
    }
}