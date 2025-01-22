using Microsoft.Extensions.Logging;
using Moq;
using TagsCloudContainerCore.Models;
using TagsCloudContainerCore.Models.Graphics;
using TagsCloudContainerCore.Renderer;

namespace TagsCloudTests;

[TestFixture]
public class RendererTests
{
    private Mock<ILogger<IRenderer>> _loggerMock;
    private Renderer _renderer;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<IRenderer>>();
        _renderer = new Renderer(_loggerMock.Object);
    }

    [Test]
    public void RenderingScale_ShouldThrowException_WhenSetToZeroOrNegative()
    {
        var zeroFunc = () => new RendererConfig { RenderingScale = 0 };
        var negativeFunc = () => new RendererConfig { RenderingScale = -1 };

        zeroFunc.Should().Throw<ArgumentOutOfRangeException>();
        negativeFunc.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void DrawTags_WithEmptyCollection_ShouldReturnSuccessResultWithEmptyImage()
    {
        var tags = Array.Empty<Tag>();

        var result = _renderer.DrawTags(tags);

        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void DrawTags_ShouldReturnSuccessResultWithCorrectImage_WhenSingleTag()
    {
        var tag = new Tag
        {
            Text = "",
            FontSize = 12,
            BBox = new Rectangle(0, 0, 100, 20)
        };

        var result = _renderer.DrawTags([tag]);

        result.IsSuccess.Should().BeTrue();
        var image = result.GetValueOrThrow();
        image.Width.Should().Be(100);
        image.Height.Should().Be(20);
    }

    [Test]
    public void DrawTags_ShouldReturnSuccessResultWithScaledImage()
    {
        var tag = new Tag
        {
            Text = "",
            FontSize = 12,
            BBox = new Rectangle(0, 0, 100, 20)
        };
        _renderer = new Renderer(_loggerMock.Object) { Config = new RendererConfig { RenderingScale = 2 } };

        var result = _renderer.DrawTags([tag]);

        result.IsSuccess.Should().BeTrue();
        var image = result.GetValueOrThrow();
        image.Width.Should().Be(200);
        image.Height.Should().Be(40);
    }

    [Test]
    public void DrawTags_ShouldReturnSuccessResultWithCorrectBounds_WithMultipleTags()
    {
        var tags = new[]
        {
            new Tag { Text = "", FontSize = 12, BBox = new Rectangle(0, 0, 50, 20) },
            new Tag { Text = "", FontSize = 12, BBox = new Rectangle(60, 0, 110, 20) }
        };

        var result = _renderer.DrawTags(tags);

        result.IsSuccess.Should().BeTrue();
        var image = result.GetValueOrThrow();
        image.Width.Should().Be(110);
        image.Height.Should().Be(20);
    }

    [Test]
    public void DrawTags_ShouldReturnSuccessResultWithImage_WhenTagsHaveNegativeCoordinates()
    {
        var tag = new Tag
        {
            Text = "",
            FontSize = 12, 
            BBox = new Rectangle(-50, -50, 50, 50)
        };

        var result = _renderer.DrawTags([tag]);

        result.IsSuccess.Should().BeTrue();
        var image = result.GetValueOrThrow();
        image.Width.Should().Be(100);
        image.Height.Should().Be(100);
    }
}