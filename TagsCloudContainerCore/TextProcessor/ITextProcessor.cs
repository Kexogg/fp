namespace TagsCloudContainerCore.TextProcessor;

public interface ITextProcessor<TConfig> : ITextProcessor
{
    TConfig Config { get; init; }
}

public interface ITextProcessor
{
    Result<Dictionary<string, double>> ProcessText(string word);
}

