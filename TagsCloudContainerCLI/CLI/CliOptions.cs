using CommandLine;

namespace TagsCloudContainerCLI.CLI;

public class CliOptions
{
    [Option('d', "demo", Required = false, HelpText = "Run the application in demo mode.")]
    public bool Demo { get; set; }

    [Option('f', "file", Required = false, HelpText = "Specify the file path.")]
    public string File { get; set; }

    [Option('o', "output", Required = false, HelpText = "Specify the output path.")]
    public string? Output { get; set; }

    [Option("font", Required = false, Default = "Arial", HelpText = "Specify the font family.")]
    public string FontFamily { get; set; }

    [Option("scale", Required = false, Default = 1, HelpText = "Specify the render scale.")]
    public float RenderScale { get; set; }

    [Option("max-words", Required = false, Default = 100, HelpText = "Maximum number of words to include.")]
    public int MaxWords { get; set; }

    [Option("min-font", Required = false, Default = 12f, HelpText = "Minimum font size.")]
    public float MinFontSize { get; set; }

    [Option("max-font", Required = false, Default = 48f, HelpText = "Maximum font size.")]
    public float MaxFontSize { get; set; }

    [Option("spacing", Required = false, Default = 0.1f, HelpText = "Space between words.")]
    public float LayoutSpacing { get; set; }

    [Option("radius", Required = false, Default = 0, HelpText = "Initial radius from center.")]
    public double InitialRadius { get; set; }

    [Option("excluded-words", Required = false, Default = "", HelpText = "Words to exclude.")]
    public string ExcludedWords { get; set; }

    [Option("excluded-parts", Required = false, Default = "PART,ADV,PR,CONJ,ANUM,APRO,SPRO,NUM", HelpText = "Parts of speech to exclude.")]
    public string ExcludedPartsOfSpeech { get; set; }
    
    [Option("sort", Required = false, Default = "Descending", HelpText = "Sort order (Ascending/Descending/Random).")]
    public string SortOrder { get; set; }
    
    [Option("background", Required = false, Default = "#000000", HelpText = "Background color in HEX.")]
    public string BackgroundColor { get; set; }
    
    [Option("foreground", Required = false, Default = "#FFFFFF", HelpText = "Foreground color in HEX.")]
    public string ForegroundColor { get; set; }
}