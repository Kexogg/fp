using Microsoft.Extensions.Logging;
using TagsCloudContainerCore.TextProcessor.MyStem;

namespace TagsCloudContainerCore.TextProcessor;

public class MyStemTextProcessor : ITextProcessor<MyStemTextProcessorConfig>
{
    private readonly ILogger<MyStemTextProcessor> _logger;

    public MyStemTextProcessorConfig Config { get; init; } = new();

    public MyStemTextProcessor(ILogger<MyStemTextProcessor> logger)
    {
        _logger = logger;
        _myStemWrapper = new MyStemWrapper();
    }

    private readonly MyStemWrapper _myStemWrapper;


    public Result<Dictionary<string, double>> ProcessText(string text)
    {
        var words = text.Split();
        _logger.LogInformation("Got {n} words", words.Length);
        _logger.LogInformation("Start processing text with MyStem");
        return _myStemWrapper.StartProcess()
            .Then(_ => GetWeightedWords(words))
            .Then(r =>
            {
                _myStemWrapper.Dispose();
                _logger.LogInformation("Finished processing text. Got {n} weighted words, excluded {e}", r.Count,
                    words.Distinct().Count() - r.Count);
                return ProcessWeightedWords(r);
            });
    }

    private static string RemoveSpecialChars(string word)
    {
        var wordChars = word.ToCharArray();
        for (var i = 0; i < wordChars.Length; i++)
        {
            if (!char.IsLetter(wordChars[i]))
            {
                wordChars[i] = ' ';
            }

            wordChars[i] = char.ToLower(wordChars[i]);
        }

        return new string(wordChars);
    }

    private Result<Dictionary<string, double>> GetWeightedWords(string[] words)
    {
        var weightedWords = new Dictionary<string, double>();
        foreach (var word in words)
        {
            if (word.Any(char.IsDigit)) continue;

            var wordWithoutSpecialChars = RemoveSpecialChars(word);

            if (string.IsNullOrWhiteSpace(wordWithoutSpecialChars))
            {
                continue;
            }

            var result = _myStemWrapper.ProcessWord(wordWithoutSpecialChars);
            if (!result.IsSuccess)
            {
                return Result.Fail<Dictionary<string, double>>(result.Error);
            }

            var processedWord = result.Value;
            
            if (processedWord == null)
            {
                continue;
            }

            var excludedByConfig = Config.ExcludedPartsOfSpeech.Contains(processedWord.PartOfSpeech) ||
                           Config.ExcludedWords.Contains(processedWord.NormalForm);
            if (excludedByConfig)
            {
                continue;
            }

            if (weightedWords.TryGetValue(processedWord.NormalForm, out var value))
            {
                weightedWords[processedWord.NormalForm] = ++value;
            }
            else
            {
                weightedWords.Add(processedWord.NormalForm, 1);
            }
        }

        return weightedWords;
    }

    private Dictionary<string, double> ProcessWeightedWords(Dictionary<string, double> weightedWords)
    {
        weightedWords = weightedWords
            .OrderByDescending(pair => pair.Value)
            .Take(Config.MaxWordsCount)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        weightedWords = Config.SortOrder switch
        {
            SortOrder.Ascending => weightedWords.OrderBy(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value),
            SortOrder.Descending => weightedWords,
            SortOrder.Random => weightedWords.OrderBy(_ => Guid.NewGuid())
                .ToDictionary(pair => pair.Key, pair => pair.Value),
            _ => weightedWords
        };
        return weightedWords;
    }
}

public enum SortOrder
{
    Ascending,
    Descending,
    Random
}