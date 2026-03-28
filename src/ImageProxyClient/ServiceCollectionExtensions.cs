namespace ImageProxyClient;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers ImageProxyClient services.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configure">Configuration delegate.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddImageProxyClient(
        this IServiceCollection services,
        Action<ImgproxyOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        services.Configure(configure);
        services.AddSingleton<IImgproxyClient, ImgproxyClient>();

        return services;
    }

    /// <summary>
    /// Registers ImageProxyClient services from configuration section.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Configuration section.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddImageProxyClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<ImgproxyOptions>(configuration);
        services.AddSingleton<IImgproxyClient, ImgproxyClient>();

        return services;
    }

    /// <summary>
    /// Registers ImageProxyClient services with startup validation.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configure">Configuration delegate.</param>
    /// <param name="validateOnStart">Whether to validate configuration on startup.</param>
    /// <returns>Service collection.</returns>
    public static IServiceCollection AddImageProxyClient(
        this IServiceCollection services,
        Action<ImgproxyOptions> configure,
        bool validateOnStart)
    {
        ArgumentNullException.ThrowIfNull(configure);

        if (validateOnStart)
        {
            services.AddOptions<ImgproxyOptions>()
                .Configure(configure)
                .Validate(options =>
                {
                    options.Validate();
                    return true;
                }, "Invalid Imgproxy configuration");
        }
        else
        {
            services.Configure(configure);
        }

        services.AddSingleton<IImgproxyClient, ImgproxyClient>();

        return services;
    }
}
