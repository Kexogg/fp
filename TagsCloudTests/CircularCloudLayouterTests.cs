using TagsCloudContainerCore.Layouter;
using TagsCloudContainerCore.Models.Graphics;
using TagsCloudContainerCore.FontManager;
using Moq;

namespace TagsCloudTests;

[TestFixture]
public class CircularCloudLayouterTests
{
    private Mock<IFontManager> _fontManagerMock;
    private const float MinFontSize = 12f;
    private const float MaxFontSize = 48f;
    private const double SpiralStep = 0.1;
    private CircularCloudLayouterConfig _config;

    [SetUp]
    public void SetUp()
    {
        _fontManagerMock = new Mock<IFontManager>();
        _config = new CircularCloudLayouterConfig
        {
            MinFontSize = MinFontSize,
            MaxFontSize = MaxFontSize,
            SpiralStep = SpiralStep
        };
        
        _fontManagerMock
            .Setup(f => f.MeasureString(It.IsAny<string>(), It.IsAny<float>()))
            .Returns((string _, float _) => Result.Ok(new Size(10, 10)));
    }

    [Test]
    public void LayoutTags_ReturnsFailure_WhenEmptyDictionary()
    {
        var layouter = new CircularCloudLayouter(_config, _fontManagerMock.Object);
        var result = layouter.LayoutTags(new Dictionary<string, double>());
        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void LayoutTags_ReturnsSuccessWithOneTag_OnSingleWord()
    {
        var layouter = new CircularCloudLayouter(_config, _fontManagerMock.Object);
        var words = new Dictionary<string, double> { { "1", 1.0 } };
        
        var result = layouter.LayoutTags(words);
        
        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().HaveCount(1);
        result.GetValueOrThrow()[0].Text.Should().Be("1");
        result.GetValueOrThrow()[0].FontSize.Should().Be(MinFontSize);
    }

    [Test]
    public void LayoutTags_ReturnsTagsThatDontOverlap()
    {
        var layouter = new CircularCloudLayouter(_config, _fontManagerMock.Object);
        var words = new Dictionary<string, double> 
        { 
            { "1", 1.0 },
            { "2", 2.0 },
            { "3", 3.0 }
        };
        
        var result = layouter.LayoutTags(words);
        
        result.IsSuccess.Should().BeTrue();
        var tags = result.GetValueOrThrow();
        for (var i = 0; i < tags.Length; i++)
        {
            for (var j = i + 1; j < tags.Length; j++)
            {
                tags[i].BBox.IntersectsWith(tags[j].BBox).Should().BeFalse();
            }
        }
    }

    [Test]
    public void LayoutTags_DifferentWeights_AffectFontSizeCorrectly()
    {
        var layouter = new CircularCloudLayouter(_config, _fontManagerMock.Object);
        var words = new Dictionary<string, double>
        {
            { "1", 1.0 },
            { "2", 2.0 }
        };

        var result = layouter.LayoutTags(words);

        result.IsSuccess.Should().BeTrue();
        var smallTag = result.GetValueOrThrow().First(t => t.Text == "1");
        var largeTag = result.GetValueOrThrow().First(t => t.Text == "2");
        smallTag.FontSize.Should().Be(MinFontSize);
        largeTag.FontSize.Should().Be(MaxFontSize);
    }

    [Test]
    public void Rectangles_ReturnsAllRectangles_AfterLayouting()
    {
        var layouter = new CircularCloudLayouter(_config, _fontManagerMock.Object);
        var words = new Dictionary<string, double> { { "1", 1.0 }, { "2", 2.0 } };
        
        var result = layouter.LayoutTags(words);
        
        result.IsSuccess.Should().BeTrue();
        layouter.Rectangles.Should().HaveCount(2);
        layouter.Rectangles.Should().Contain(result.GetValueOrThrow()[0].BBox);
        layouter.Rectangles.Should().Contain(result.GetValueOrThrow()[1].BBox);
    }

    [Test]
    public void LayoutTags_UsesMinFontSize_WhenSameWeight()
    {
        var layouter = new CircularCloudLayouter(_config, _fontManagerMock.Object);
        var words = new Dictionary<string, double>
        {
            { "1", 1.0 },
            { "2", 1.0 }
        };

        var result = layouter.LayoutTags(words);

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().AllSatisfy(tag => tag.FontSize.Should().Be(MinFontSize));
    }

    [Test]
    public void LayoutTags_ReturnsFailure_WhenFontManagerFails()
    {
        _fontManagerMock
            .Setup(f => f.MeasureString(It.IsAny<string>(), It.IsAny<float>()))
            .Returns(Result.Fail<Size>("Font measurement failed"));

        var layouter = new CircularCloudLayouter(_config, _fontManagerMock.Object);
        var words = new Dictionary<string, double> { { "1", 1.0 } };

        var result = layouter.LayoutTags(words);

        result.IsSuccess.Should().BeFalse();
    }
}