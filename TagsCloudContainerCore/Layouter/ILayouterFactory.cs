namespace TagsCloudContainerCore.Layouter;

public interface ILayouterFactory
{
    Result<ILayouter> Create();
}

public interface ILayouterFactory<TConfig> : ILayouterFactory
{
    TConfig Config { get; init; }
}