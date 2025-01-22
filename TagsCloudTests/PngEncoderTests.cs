using SkiaSharp;
using TagsCloudContainerCore.ImageEncoders;

namespace TagsCloudTests;

[TestFixture]
public class PngEncoderTests
{
    private PngEncoder _encoder;
    private SKImage _testImage;

    [SetUp]
    public void Setup()
    {
        _encoder = new PngEncoder();
        using var surface = SKSurface.Create(new SKImageInfo(1, 1));
        _testImage = surface.Snapshot();
    }

    [Test]
    public void Encode_ValidImage_ReturnsSuccessResultWithNonEmptyByteArray()
    {
        var result = _encoder.Encode(_testImage);

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().NotBeEmpty();
    }

    [Test]
    public void Encode_ValidImage_ReturnsPngFormat()
    {
        var result = _encoder.Encode(_testImage);
        result.IsSuccess.Should().BeTrue();
        var bytes = result.GetValueOrThrow();

        // magic numbers
        
        var pngHeader = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
        var pngFooter = new byte[] { 73, 69, 78, 68, 174, 66, 96, 130 };
        bytes.Take(8).Should().BeEquivalentTo(pngHeader);
        bytes[^8..].Should().BeEquivalentTo(pngFooter);
    }

    [Test]
    public void Encode_NullImage_ReturnsFailureResult()
    {
        var result = _encoder.Encode(null!);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [TearDown]
    public void Cleanup()
    {
        _testImage.Dispose();
    }
}