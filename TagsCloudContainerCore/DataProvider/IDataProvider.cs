namespace TagsCloudContainerCore.DataProvider;

public interface IDataProvider<TConfig> : IDataProvider
{
    TConfig Config { get; init; }
}

public interface IDataProvider
{
    Result<string> GetData(byte[] data);
}