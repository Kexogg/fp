using SkiaSharp;

namespace TagsCloudContainerCore.ImageEncoders;

public class PngEncoder : IImageEncoder
{
    public Result<byte[]> Encode(SKImage image)
    {
        return Result.Of(() => image.Encode(SKEncodedImageFormat.Png, 100).ToArray()).RefineError("Failed to encode image");
    }
}