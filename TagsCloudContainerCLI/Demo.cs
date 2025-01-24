using System.Text;
using Microsoft.Extensions.Logging;
using TagsCloudContainerCore.DataProvider;
using TagsCloudContainerCore.Facade;
using TagsCloudContainerCore.FontManager;
using TagsCloudContainerCore.ImageEncoders;
using TagsCloudContainerCore.Layouter;
using TagsCloudContainerCore.Models.Graphics;
using TagsCloudContainerCore.Renderer;
using TagsCloudContainerCore.TextProcessor;

namespace TagsCloudContainerCLI;

public class Demo
{
    private ITagCloudFactory _cloudFactory;
    private ILogger<Demo> _logger;

    public Demo(ITagCloudFactory cloudFactory, ILogger<Demo> logger)
    {
        _cloudFactory = cloudFactory;
        _logger = logger;
    }

    public Result<None> Generate()
    {
        _logger.LogInformation("Generating random clouds");
        Directory.CreateDirectory("results");

        return GenerateRandomCloud(10)
            .Then(_ => GenerateRandomCloud(50))
            .Then(_ => GenerateRandomCloud(100));
    }

    private Result<None> GenerateRandomCloud(int count)
    {
        _logger.LogInformation("Generating cloud with {Count} words", count);
        return _cloudFactory.Create(builder => builder
                .UseDataProvider<FileDataProvider>()
                .UseWordProcessor<MyStemTextProcessor, MyStemTextProcessorConfig>(
                    new MyStemTextProcessorConfig
                    {
                        ExcludedWords = ["тест"],
                        ExcludedPartsOfSpeech = []
                    })
                .UseFontManager<FontManager, FontManagerConfig>(
                    new FontManagerConfig
                    {
                        Font = new Font("Arial")
                    }
                )
                .UseLayouter<CircularCloudLayouterFactory, CircularCloudLayouterConfig>(
                    new CircularCloudLayouterConfig
                    {
                        SpiralStep = 0.5,
                        InitialRadius = 100
                    })
                .UseRenderer<Renderer, RendererConfig>(new RendererConfig
                    {
                        BackgroundColor = new Color(200, 200, 255),
                        RenderingScale = 5,
                        TextColor = new Color(0, 0, 100)
                    }
                )
                .UseImageEncoder<PngEncoder>())
            .Then(r => r.FromString(GenerateRandomString(count)))
            .Then(imageBytes => File.WriteAllBytes($"results/random_cloud_{count}.png", imageBytes));
    }

    private static string GenerateRandomString(int count)
    {
        var randomString =
            ("Lorem ipsum — классический текст-«рыба» (условный, зачастую бессмысленный текст-заполнитель, вставляемый в макет страницы, используемый для образца шрифта и текста, поля размещения на странице). "
             + "Используется для демонстрации элементов графики в документах или презентациях, без отвлечения внимания на содержимое. "
             + "Слова «Lorem ipsum» берут начало от латинского слова и начинаются с «dolorem ipsum», что означает «боль сама по себе». "
             + "Текст не имеет смысла, но по своей структуре напоминает настоящий текст. "
             + "Слова и предложения в «Lorem ipsum» обычно не повторяются, что делает его более правдоподобным. ")
                .Split();
        var random = new Random();
        var sb = new StringBuilder();
        for (var i = 0; i < count - 1; i++)
        {
            sb.Append(randomString[random.Next(randomString.Length)]);
            sb.Append(' ');
        }

        return sb.ToString();
    }
}