using SkiaSharp;

namespace TagsCloudContainerCore.ImageEncoders;

public interface IImageEncoder
{
    Result<byte[]> Encode(SKImage image);
}