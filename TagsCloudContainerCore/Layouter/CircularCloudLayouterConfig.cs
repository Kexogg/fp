namespace TagsCloudContainerCore.Layouter;

public record CircularCloudLayouterConfig
{
    public double SpiralStep { get; set; } = 0.1;
    public float MinFontSize { get; set; } = 12.0f;
    public float MaxFontSize { get; set; } = 48.0f;
    public double InitialRadius { get; set; } = 0;
}