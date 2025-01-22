namespace TagsCloudContainerCore.Facade;

public interface ITagCloudFactory
{
    Result<ITagCloud> Create(Action<TagCloudBuilder> configure);
}