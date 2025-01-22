using Microsoft.Extensions.Logging;
using Moq;
using TagsCloudContainerCore.Models;
using TagsCloudContainerCore.TextProcessor;

namespace TagsCloudTests;

public class MyStemTextProcessorTests
{
    private readonly MyStemTextProcessor _myStemTextProcessor;
    
    public MyStemTextProcessorTests()
    {
        var mockLogger = new Mock<ILogger<MyStemTextProcessor>>();
        _myStemTextProcessor = new MyStemTextProcessor(mockLogger.Object);
    }

    [Test]
    public void MyStemWordProcessor_ShouldProcessWord()
    {
        var result = _myStemTextProcessor.ProcessText("слово");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, double>("слово", 1));
    }

    [Test]
    public void MyStemWordProcessor_ShouldProcessMultipleWords()
    {
        var result = _myStemTextProcessor.ProcessText("слово слова");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().HaveCount(1);
        result.GetValueOrThrow().Should().ContainSingle(pair => pair.Key == "слово" && pair.Value == 2);
    }

    [Test]
    public void MyStemWordProcessor_ShouldProcessWordWithExcludedPartOfSpeech()
    {
        _myStemTextProcessor.Config.ExcludedPartsOfSpeech = [PartOfSpeech.S];
        var result = _myStemTextProcessor.ProcessText("слово");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().BeEmpty();
    }

    [Test]
    public void MyStemWordProcessor_ShouldProcessWordWithExcludedWord()
    {
        _myStemTextProcessor.Config.ExcludedWords = ["слово"];
        var result = _myStemTextProcessor.ProcessText("слово");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().BeEmpty();
    }

    [Test]
    public void MyStemWordProcessor_ShouldProcessLongText()
    {
        var result = _myStemTextProcessor.ProcessText("Это текст, который нужно обработать. Текст, текст, текст.");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().HaveCount(4);
        result.GetValueOrThrow().Should().Contain(pair => pair.Key == "это" && pair.Value == 1);
        result.GetValueOrThrow().Should().Contain(pair => pair.Key == "текст" && pair.Value == 4);
        result.GetValueOrThrow().Should().Contain(pair => pair.Key == "который" && pair.Value == 1);
        result.GetValueOrThrow().Should().Contain(pair => pair.Key == "обрабатывать" && pair.Value == 1);
    }
    
    [Test]
    public void MyStemWordProcessor_ShouldProcessEmptyString()
    {
        var result = _myStemTextProcessor.ProcessText("");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().BeEmpty();
    }
    
    [Test]
    public void MyStemWordProcessor_ShouldProcessInvalidString()
    {
        var result = _myStemTextProcessor.ProcessText("123");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().BeEmpty();
    }
}