using TagsCloudContainerCore.FontManager;

namespace TagsCloudContainerCore.Layouter;

public class CircularCloudLayouterFactory : ILayouterFactory<CircularCloudLayouterConfig>
{
    public CircularCloudLayouterConfig Config { get; init; }
    private readonly IFontManager _fontManager;

    public CircularCloudLayouterFactory(IFontManager fontManager)
    {
        _fontManager = fontManager;
    }

    public Result<ILayouter> Create()
    {
        return Result.Of<ILayouter>(() => new CircularCloudLayouter(Config, _fontManager));
    }
}