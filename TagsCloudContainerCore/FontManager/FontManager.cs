using TagsCloudContainerCore.Models.Graphics;

namespace TagsCloudContainerCore.FontManager;

public class FontManager : IFontManager<FontManagerConfig>
{
    public FontManagerConfig Config { get; init; }

    public Result<Size> MeasureString(string text, float fontSize)
    {
        try
        {
            var font = new Font(Config.Font.Typeface, fontSize);
            var measuredSize = new Size(font.MeasureText(text), fontSize);
            return Result.Ok(measuredSize);
        }
        catch (Exception ex)
        {
            return Result.Fail<Size>(ex.Message);
        }
    }
}