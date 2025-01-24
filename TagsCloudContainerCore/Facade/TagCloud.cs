using System.Text;
using TagsCloudContainerCore.DataProvider;
using TagsCloudContainerCore.ImageEncoders;
using TagsCloudContainerCore.Layouter;
using TagsCloudContainerCore.Renderer;
using TagsCloudContainerCore.TextProcessor;

namespace TagsCloudContainerCore.Facade;

public class TagCloud : ITagCloud
{
    private readonly ITextProcessor _textProcessor;
    private readonly ILayouterFactory _layouterFactory;
    private readonly IRenderer _renderer;
    private readonly IImageEncoder _imageEncoder;
    private readonly IDataProvider _dataProvider;

    public TagCloud(
        ITextProcessor textProcessor,
        ILayouterFactory layouterFactory,
        IRenderer renderer,
        IImageEncoder imageEncoder,
        IDataProvider dataProvider)
    {
        _textProcessor = textProcessor;
        _layouterFactory = layouterFactory;
        _renderer = renderer;
        _imageEncoder = imageEncoder;
        _dataProvider = dataProvider;
    }

    public Result<byte[]> FromString(string data)
    {
        return ProcessString(data);
    }

    public Result<byte[]> FromFile(string filePath)
    {
        return Result.Of(() => File.ReadAllBytes(filePath))
            .Then(_dataProvider.GetData)
            .Then(ProcessString);
    }

    public Result<byte[]> FromBytes(byte[] data)
    {
        return Result.Of(() => Encoding.UTF8.GetString(data))
            .Then(ProcessString);
    }

    private Result<byte[]> ProcessString(string data)
    {
        var layouter = _layouterFactory.Create();
        if (!layouter.IsSuccess)
            return Result.Fail<byte[]>(layouter.Error);
        return _textProcessor.ProcessText(data)
            .Then(layouter.Value.LayoutTags)
            .Then(_renderer.DrawTags)
            .Then(_imageEncoder.Encode);
    }
}