using TagsCloudContainerCore.Models;

namespace TagsCloudContainerCore.TextProcessor;

public record MyStemTextProcessorConfig
{
    public PartOfSpeech[] ExcludedPartsOfSpeech { get; set; } =
        { PartOfSpeech.PART, PartOfSpeech.ADV, PartOfSpeech.PR, PartOfSpeech.CONJ };

    public string[] ExcludedWords { get; set; } = [];

    public SortOrder SortOrder { get; set; } = SortOrder.Descending;

    public int MaxWordsCount { get; set; } = 50;
}