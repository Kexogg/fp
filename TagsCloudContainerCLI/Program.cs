using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Autofac.DependencyInjection;
using TagsCloudContainerCLI.CLI;
using TagsCloudContainerCore.Facade;

namespace TagsCloudContainerCLI;

internal class Program
{
    private static ParserResult<CliOptions>? _parseResult;

    private static void Main(string[] args)
    {
        _parseResult = Parser.Default.ParseArguments<CliOptions>(args);

        if (_parseResult.Tag == ParserResultType.NotParsed)
        {
            Environment.Exit(1);
        }
        
        Result.Of(() => BuildHost(args))
            .Then(host =>
            {
                using var scope = host.Services.CreateScope();
                return ProcessArguments(scope.ServiceProvider);
            })
            .OnFail(error =>
            {
                Log.Fatal($"Application error: {error}");
                Environment.Exit(1);
            });
    }

    private static IHost BuildHost(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureServices(ConfigureServices)
            .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
            .Build();

    private static void ConfigureServices(HostBuilderContext _, IServiceCollection services)
    {
        services.AddOptions();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
    }

    private static void ConfigureContainer(ContainerBuilder builder)
    {
        ConfigureSerilog(builder);
        ConfigureTagCloud(builder);
        builder.RegisterInstance(CreateTagCloudConfig(_parseResult!.Value)).As<TagCloudConfig>();
    }

    private static void ConfigureSerilog(ContainerBuilder builder)
    {
        builder.RegisterSerilog(new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.log", rollingInterval: RollingInterval.Day));
    }

    private static void ConfigureTagCloud(ContainerBuilder builder)
    {
        builder.RegisterType<TagCloudFactory>()
            .As<ITagCloudFactory>()
            .InstancePerLifetimeScope();

        builder.RegisterType<FileMode>().AsSelf();
        builder.RegisterType<Demo>().AsSelf().SingleInstance();
    }

    private static Result<None> ProcessArguments(IServiceProvider services)
    {
        return _parseResult!
            .MapResult(
                opts => HandleValidOptions(opts, services),
                errors => Result.Fail<None>($"Invalid command line arguments: {errors}")
            );
    }

    private static TagCloudConfig CreateTagCloudConfig(CliOptions options) =>
        new()
        {
            FontFamily = options.FontFamily,
            RenderScale = options.RenderScale,
            MaxWords = options.MaxWords,
            MinFontSize = options.MinFontSize,
            MaxFontSize = options.MaxFontSize,
            LayoutSpacing = options.LayoutSpacing,
            InitialRadius = options.InitialRadius,
            ExcludedWords = options.ExcludedWords.Split(","),
            ExcludedPartsOfSpeech = options.ExcludedPartsOfSpeech.Split(","),
            SortOrder = options.SortOrder,
            ForegroundColor = options.ForegroundColor,
            BackgroundColor = options.BackgroundColor,
        };


    private static Result<None> HandleValidOptions(CliOptions opts, IServiceProvider services)
    {
        return CliHandler.ValidateOptions(opts)
            .Then(_ =>
            {
                if (opts.Demo)
                {
                    var demo = services.GetRequiredService<Demo>();
                    return demo.Generate();
                }

                if (string.IsNullOrEmpty(opts.File))
                    return Result.Fail<None>("No file specified");
                var fileMode = services.GetRequiredService<FileMode>();
                var outputPath = opts.Output ?? $"{opts.File}.png";
                return fileMode.Generate(opts.File, outputPath);
            });
    }
}