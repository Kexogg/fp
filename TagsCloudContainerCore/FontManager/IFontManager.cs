using TagsCloudContainerCore.Models.Graphics;

namespace TagsCloudContainerCore.FontManager;

public interface IFontManager<TConfig> : IFontManager
{
    TConfig Config { get; init; }
}

public interface IFontManager
{
    Result<Size> MeasureString(string text, float fontSize);
}