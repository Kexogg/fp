using DocumentFormat.OpenXml.Packaging;
using Microsoft.Extensions.Logging;

namespace TagsCloudContainerCore.DataProvider;

public class OpenXmlDocumentsProvider : IDataProvider
{
    private readonly ILogger<IDataProvider> _logger;

    public OpenXmlDocumentsProvider(ILogger<IDataProvider> logger)
    {
        _logger = logger;
    }

    public Result<string> GetData(byte[] data)
    {
        _logger.LogInformation("Reading data with OpenXmlDocumentsProvider");
        using var stream = new MemoryStream(data);

        try
        {
            using var doc = WordprocessingDocument.Open(stream, false);
            if (doc.MainDocumentPart?.Document.Body != null)
            {
                var result = doc.MainDocumentPart.Document.Body.InnerText;
                _logger.LogInformation("Read {w} characters from document", result.Length);
                return result;
            }
            return Result.Fail<string>("No body found in document");
        }
        catch (Exception ex) when (ex is OpenXmlPackageException or InvalidDataException)
        {
            return Result.Fail<string>(ex.ToString()).RefineError("Failed to read document");
        }
    }
}