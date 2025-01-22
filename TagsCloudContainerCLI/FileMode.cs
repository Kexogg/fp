using Microsoft.Extensions.Logging;
using TagsCloudContainerCLI.CLI;
using TagsCloudContainerCore.DataProvider;
using TagsCloudContainerCore.Facade;
using TagsCloudContainerCore.FontManager;
using TagsCloudContainerCore.ImageEncoders;
using TagsCloudContainerCore.Layouter;
using TagsCloudContainerCore.Models;
using TagsCloudContainerCore.Models.Graphics;
using TagsCloudContainerCore.Renderer;
using TagsCloudContainerCore.TextProcessor;


namespace TagsCloudContainerCLI;

public class FileMode
{
    private readonly ILogger<FileMode> _logger;
    private readonly ITagCloudFactory _cloudFactory;
    private readonly TagCloudConfig _config;

    public FileMode(ILogger<FileMode> logger, ITagCloudFactory cloudFactory, TagCloudConfig config)
    {
        _logger = logger;
        _cloudFactory = cloudFactory;
        _config = config;
    }

    public Result<None> Generate(string filePath, string outputPath)
    {
        _logger.LogInformation("Generating tag cloud to {Path}", outputPath);

        return _cloudFactory.Create(builder => builder
                .GuessDataProvider(Path.GetExtension(filePath))
                .UseWordProcessor<MyStemTextProcessor, MyStemTextProcessorConfig>(new MyStemTextProcessorConfig
                {
                    MaxWordsCount = _config.MaxWords,
                    ExcludedWords = _config.ExcludedWords,
                    ExcludedPartsOfSpeech = _config.ExcludedPartsOfSpeech
                        .Select(pos =>
                        {
                            if (Enum.TryParse<PartOfSpeech>(pos, true, out var parsedPos))
                            {
                                return parsedPos;
                            }

                            _logger.LogWarning("Invalid PartOfSpeech value: {Value}", pos);
                            return (PartOfSpeech?)null;
                        })
                        .Where(pos => pos.HasValue)
                        .Select(pos => pos!.Value)
                        .ToArray(),
                    SortOrder = Enum.Parse<SortOrder>(_config.SortOrder, true)
                })
                .UseFontManager<FontManager, FontManagerConfig>(new FontManagerConfig
                {
                    Font = new Font(_config.FontFamily)
                })
                .UseLayouter<CircularCloudLayouterFactory, CircularCloudLayouterConfig>(new CircularCloudLayouterConfig
                {
                    MaxFontSize = _config.MaxFontSize,
                    MinFontSize = _config.MinFontSize,
                    SpiralStep = _config.LayoutSpacing,
                    InitialRadius = _config.InitialRadius
                })
                .UseRenderer<Renderer, RendererConfig>(new RendererConfig
                {
                    TagFont = new Font(_config.FontFamily),
                    RenderingScale = _config.RenderScale,
                    BackgroundColor = new Color(_config.BackgroundColor),
                    TextColor = new Color(_config.ForegroundColor)
                })
                .GuessEncoder(Path.GetExtension(outputPath)))
            .Then(r => r.FromFile(filePath)
                .RefineError("Failed to generate tag cloud"))
            .Then(imageBytes => SaveTo(outputPath, imageBytes));
    }

    private Result<None> SaveTo(string outputPath, byte[] imageBytes)
    {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                return Result.Fail<None>($"Failed to create directory: {ex.Message}");
            }
        }

        var fullpath = Path.GetFullPath(outputPath);
        _logger.LogInformation("Saving tag cloud to {Path}", fullpath);
        try
        {
            File.WriteAllBytes(outputPath, imageBytes);
        }
        catch (Exception e)
        {
            return Result.Fail<None>($"Failed to save image: {e.Message}");
        }

        return Result.Ok();
    }
}

public static class BuilderExtensions
{
    public static TagCloudBuilder GuessDataProvider(this TagCloudBuilder b, string ext)
    {
        return ext switch
        {
            ".docx" => b.UseDataProvider<OpenXmlDocumentsProvider>(),
            ".doc" => b.UseDataProvider<OpenXmlDocumentsProvider>(),
            ".ppt" => b.UseDataProvider<OpenXmlSlidesProvider>(),
            ".pptx" => b.UseDataProvider<OpenXmlSlidesProvider>(),
            ".txt" => b.UseDataProvider<FileDataProvider>(),
            _ => b.UseDataProvider<FileDataProvider>(),
        };
    }

    public static TagCloudBuilder GuessEncoder(this TagCloudBuilder b, string ext)
    {
        return ext switch
        {
            ".png" => b.UseImageEncoder<PngEncoder>(),
            ".jpeg" => b.UseImageEncoder<JpegEncoder>(),
            ".jpg" => b.UseImageEncoder<JpegEncoder>(),
            _ => b.UseImageEncoder<PngEncoder>(),
        };
    }
}