using System.Text;
using Microsoft.Extensions.Logging;

namespace TagsCloudContainerCore.DataProvider;

public class FileDataProvider : IDataProvider
{
    private readonly ILogger<IDataProvider> _logger;

    public FileDataProvider(ILogger<IDataProvider> logger)
    {
        _logger = logger;
    }

    public Result<string> GetData(byte[] data)
    {
        _logger.LogInformation("Reading data with FileDataProvider");
        var text = Encoding.UTF8.GetString(data);
        _logger.LogInformation("Read {n} characters from file", data.Length);
        return text;
    }
}