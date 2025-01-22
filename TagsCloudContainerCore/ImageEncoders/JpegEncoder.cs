using SkiaSharp;

namespace TagsCloudContainerCore.ImageEncoders;

public class JpegEncoder : IImageEncoder
{
    public Result<byte[]> Encode(SKImage image)
    {
        return Result.Of(() => image.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray()).RefineError("Failed to encode image");
    }
}