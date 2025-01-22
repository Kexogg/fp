using TagsCloudContainerCore.DataProvider;
using TagsCloudContainerCore.FontManager;
using TagsCloudContainerCore.ImageEncoders;
using TagsCloudContainerCore.Layouter;
using TagsCloudContainerCore.Renderer;
using TagsCloudContainerCore.TextProcessor;

namespace TagsCloudContainerCore.Facade;

public class TagCloudBuilder
{
    private readonly TagCloudOptions _options = new();

    public TagCloudBuilder UseDataProvider<T>()
    {
        _options.DataProviderType = typeof(T);
        return this;
    }
    
    public TagCloudBuilder UseFontManager<T, TConfig>(TConfig? config)
    {
        _options.FontManagerType = typeof(T);
        _options.RegisterConfiguration(config);
        return this;
    } 
    
    public TagCloudBuilder UseLayouter<T, TConfig>(TConfig? config) where T : ILayouterFactory<TConfig>
    {
        _options.LayouterType = typeof(T);
        _options.RegisterConfiguration(config);
        return this;
    }
    
    public TagCloudBuilder UseWordProcessor<T, TConfig>(TConfig? config)
    {
        _options.WordProcessorType = typeof(T);
        _options.RegisterConfiguration(config);
        return this;
    }
    
    public TagCloudBuilder UseRenderer<T, TConfig>(TConfig? config) where T : IRenderer<TConfig>
    {
        _options.RendererType = typeof(T);
        _options.RegisterConfiguration(config);
        return this;
    }
    
    public TagCloudBuilder UseImageEncoder<T>()
    {
        _options.ImageEncoderType = typeof(T);
        return this;
    }

    public TagCloudBuilder RegisterService<TService, TImplementation>() where TImplementation : TService
    {
        _options.RegisterService<TService, TImplementation>();
        return this;
    }

    public TagCloudOptions Build() => _options;
}