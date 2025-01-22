using TagsCloudContainerCore.Models;
using TagsCloudContainerCore.TextProcessor.MyStem;

namespace TagsCloudTests;

public class MyStemWrapperTests
{
    private readonly MyStemWrapper _myStemWrapper = new();
    
    [SetUp]
    public void SetUp()
    {
        var result = _myStemWrapper.StartProcess();
        result.IsSuccess.Should().BeTrue();
    }
    
    [TearDown]
    public void TearDown()
    {
        _myStemWrapper.Dispose();
    }
    
    [Test]
    public void MyStemWrapper_ShouldProcessWord()
    {
        var result = _myStemWrapper.ProcessWord("слово");
        result.IsSuccess.Should().BeTrue();
        var processedWord = result.GetValueOrThrow();
        processedWord.Should().NotBeNull();
        processedWord!.NormalForm.Should().Be("слово");
        processedWord.PartOfSpeech.Should().Be(PartOfSpeech.S);
    }
    
    [Test]
    public void MyStemWrapper_ShouldProcessWord_WithPunctuation()
    {
        var result = _myStemWrapper.ProcessWord("слово,");
        result.IsSuccess.Should().BeTrue();
        var processedWord = result.GetValueOrThrow();
        processedWord.Should().NotBeNull();
        processedWord!.NormalForm.Should().Be("слово");
        processedWord.PartOfSpeech.Should().Be(PartOfSpeech.S);
    }
    
    [Test]
    public void MyStemWrapper_ShouldProcessWord_InDifferentForms()
    {
        var result = _myStemWrapper.ProcessWord("слова");
        result.IsSuccess.Should().BeTrue();
        var processedWord = result.GetValueOrThrow();
        processedWord.Should().NotBeNull();
        processedWord!.NormalForm.Should().Be("слово");
        processedWord.PartOfSpeech.Should().Be(PartOfSpeech.S);
    }
    
    [Test]
    public void MyStemWrapper_ShouldProcessWord_WithDifferentPartOfSpeech()
    {
        var result = _myStemWrapper.ProcessWord("говорю");
        result.IsSuccess.Should().BeTrue();
        var processedWord = result.GetValueOrThrow();
        processedWord.Should().NotBeNull();
        processedWord!.NormalForm.Should().Be("говорить");
        processedWord.PartOfSpeech.Should().Be(PartOfSpeech.V);
    }
    
    [Test]
    public void MyStemWrapper_ShouldProcessMultipleWords()
    {
        var result = _myStemWrapper.ProcessWord("слово");
        result.IsSuccess.Should().BeTrue();
        var processedWord = result.GetValueOrThrow();
        processedWord.Should().NotBeNull();
        processedWord!.NormalForm.Should().Be("слово");
        processedWord.PartOfSpeech.Should().Be(PartOfSpeech.S);
        
        result = _myStemWrapper.ProcessWord("слова");
        result.IsSuccess.Should().BeTrue();
        processedWord = result.GetValueOrThrow();
        processedWord.Should().NotBeNull();
        processedWord!.NormalForm.Should().Be("слово");
        processedWord.PartOfSpeech.Should().Be(PartOfSpeech.S);
        
        result = _myStemWrapper.ProcessWord("говорю");
        result.IsSuccess.Should().BeTrue();
        processedWord = result.GetValueOrThrow();
        processedWord.Should().NotBeNull();
        processedWord!.NormalForm.Should().Be("говорить");
        processedWord.PartOfSpeech.Should().Be(PartOfSpeech.V);
        
        result = _myStemWrapper.ProcessWord("говоря");
        result.IsSuccess.Should().BeTrue();
        processedWord = result.GetValueOrThrow();
        processedWord.Should().NotBeNull();
        processedWord!.NormalForm.Should().Be("говорить");
        processedWord.PartOfSpeech.Should().Be(PartOfSpeech.V);
    }

    [Test]
    public void MyStemWrapper_ShouldReturnNull_ForUnprocessableWord()
    {
        var result = _myStemWrapper.ProcessWord("ъ");
        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().BeNull();
    }

    [Test]
    public void MyStemWrapper_ShouldFail_WhenProcessNotStarted()
    {
        var wrapper = new MyStemWrapper();
        var result = wrapper.ProcessWord("слово");
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Process is not running.");
    }
}