namespace TagsCloudContainerCLI.CLI;

public record TagCloudConfig
{
    public string FontFamily { get; set; }
    public float RenderScale { get; set; }
    public int MaxWords { get; set; }
    public float MinFontSize { get; set; }
    public float MaxFontSize { get; set; }
    public float LayoutSpacing { get; set; }
    public double InitialRadius { get; set; }
    public string[] ExcludedWords { get; set; }
    public string[] ExcludedPartsOfSpeech { get; set; }
    public string SortOrder { get; set; }
    public string BackgroundColor { get; set; }
    public string ForegroundColor { get; set; }
}