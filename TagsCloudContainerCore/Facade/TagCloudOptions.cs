using TagsCloudContainerCore.DataProvider;
using TagsCloudContainerCore.FontManager;
using TagsCloudContainerCore.ImageEncoders;
using TagsCloudContainerCore.Layouter;
using TagsCloudContainerCore.Renderer;
using TagsCloudContainerCore.TextProcessor;

namespace TagsCloudContainerCore.Facade;

public class TagCloudOptions
{
    internal Dictionary<Type, Type> ServiceMap { get; } = new();
    internal Dictionary<Type, object> Configurations { get; } = new();
    
    public Type DataProviderType { get; internal set; } = typeof(IDataProvider);
    public Type FontManagerType { get; internal set; } = typeof(IFontManager);
    public Type LayouterType { get; internal set; } = typeof(ILayouterFactory);
    public Type WordProcessorType { get; internal set; } = typeof(ITextProcessor);
    public Type RendererType { get; internal set; } = typeof(IRenderer);
    public Type ImageEncoderType { get; internal set; } = typeof(IImageEncoder);

    public void RegisterService<TService, TImplementation>() where TImplementation : TService
    {
        ServiceMap[typeof(TService)] = typeof(TImplementation);
    }

    public void RegisterConfiguration<TConfig>(TConfig config)
    {
        Configurations[typeof(TConfig)] = config;
    }
}