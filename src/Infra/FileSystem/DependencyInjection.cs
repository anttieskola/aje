namespace AJE.Infra.FileSystem;

public static class DependencyInjection
{
    public static IServiceCollection AddFileSystem(
        this IServiceCollection services,
        IConfigurationRoot config)
    {
        var fileSystemConfiguration = config.GetFileSystemConfiguration();
        services.AddSingleton(fileSystemConfiguration);
        services.AddSingleton<IYleRepository, YleRepository>();
        return services;
    }

    public static FileSystemConfiguration GetFileSystemConfiguration(this IConfigurationRoot config)
    {
        return (config.GetSection(nameof(FileSystemConfiguration)).Get<FileSystemConfiguration>())
            ?? throw new PlatformException(nameof(FileSystemConfiguration));
    }

    public static Task InitializeFileSystemAsync(this IServiceProvider provider)
    {
        var configuration = provider.GetRequiredService<FileSystemConfiguration>();
        if (!Directory.Exists(configuration.RootFolder))
        {
            Directory.CreateDirectory(configuration.RootFolder);
        }
        var yleFolder = Path.Combine(configuration.RootFolder, "yle");
        if (!Directory.Exists(yleFolder))
        {
            Directory.CreateDirectory(yleFolder);
        }
        return Task.CompletedTask;
    }
}
