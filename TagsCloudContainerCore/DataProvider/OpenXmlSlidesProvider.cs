using DocumentFormat.OpenXml.Packaging;
using Microsoft.Extensions.Logging;
using System.Text;

namespace TagsCloudContainerCore.DataProvider;

public class OpenXmlSlidesProvider : IDataProvider
{
    private readonly ILogger<IDataProvider> _logger;

    public OpenXmlSlidesProvider(ILogger<IDataProvider> logger)
    {
        _logger = logger;
    }

    public Result<string> GetData(byte[] data)
    {
        _logger.LogInformation("Reading data with OpenXmlProvider");
        using var stream = new MemoryStream(data);
        try
        {
            using var doc = PresentationDocument.Open(stream, false);
            var text = new StringBuilder();
            var slides = doc.PresentationPart?.SlideParts;

            if (slides != null)
            {
                foreach (var slide in slides)
                {
                    text.AppendLine(slide.Slide.InnerText);
                }

                var result = text.ToString();
                _logger.LogInformation("Read {w} characters from presentation", result.Length);
                return result;
            }
            return Result.Fail<string>("No slides found in presentation");
        }
        catch (Exception ex) when (ex is OpenXmlPackageException or InvalidDataException)
        {
            return Result.Fail<string>(ex.ToString()).RefineError("Failed to read presentation");
        }
    }
}