using TagsCloudContainerCore.Models;

namespace TagsCloudContainerCore.Layouter;

public interface ILayouter
{
    Result<Tag[]> LayoutTags(Dictionary<string, double> words);
}