using Autofac;

namespace TagsCloudContainerCore.Facade;

public class TagCloudFactory : ITagCloudFactory
{
    private readonly ILifetimeScope _scope;

    public TagCloudFactory(ILifetimeScope scope)
    {
        _scope = scope;
    }

    public Result<ITagCloud> Create(Action<TagCloudBuilder> configure)
    {
        return Result.Of(() =>
        {
            var builder = new TagCloudBuilder();
            configure(builder);

            var options = builder.Build();
            var childScope = _scope.BeginLifetimeScope(cb => { cb.RegisterModule(new TagCloudModule(options)); });

            return childScope.Resolve<ITagCloud>();
        });
    }
}