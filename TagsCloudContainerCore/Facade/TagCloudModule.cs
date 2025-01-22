using Autofac;
using Autofac.Builder;

namespace TagsCloudContainerCore.Facade;

public class TagCloudModule : Module
{
    private readonly TagCloudOptions _options;

    public TagCloudModule(TagCloudOptions options)
    {
        _options = options;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TagCloud>()
            .AsSelf()
            .As<ITagCloud>()
            .InstancePerLifetimeScope();

        RegisterService(builder, _options.DataProviderType);
        RegisterServiceWithConfig(builder, _options.FontManagerType);
        RegisterServiceWithConfig(builder, _options.WordProcessorType);
        RegisterServiceWithConfig(builder, _options.LayouterType);
        RegisterServiceWithConfig(builder, _options.RendererType);
        RegisterService(builder, _options.ImageEncoderType);

        foreach (var (serviceType, implementationType) in _options.ServiceMap)
        {
            RegisterServiceWithConfig(builder, implementationType, serviceType);
        }
    }

    private void RegisterServiceWithConfig(ContainerBuilder builder, Type implementationType, Type? serviceType = null)
    {
        var configInterface = implementationType.GetInterfaces()
            .FirstOrDefault(type => type.IsGenericType &&
                                 type.GetProperties().Any(p => p.Name == "Config"));

        var registration = RegisterService(builder, implementationType, serviceType);

        if (configInterface == null) return;
        var configType = configInterface.GetGenericArguments()[0];
        if (_options.Configurations.TryGetValue(configType, out var config))
        {
            registration.OnActivated(@event =>
            {
                var configProperty = @event.Instance.GetType().GetProperty("Config");
                configProperty?.SetValue(@event.Instance, config);
            });
        }
    }

    private static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
        RegisterService(ContainerBuilder builder, Type implementationType, Type? serviceType = null)
    {
        var registration = builder.RegisterType(implementationType);

        if (serviceType != null)
        {
            registration.As(serviceType);
        }
        else
        {
            registration.AsImplementedInterfaces();
        }

        registration.SingleInstance();
        return registration;
    }
}