using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolarX.SERVICE.Abstractions.ICloudinaryService;
using Startup_Project.SERVICE.Services.CloudinaryServices;

namespace SolarX.SERVICE.Services.CloudinaryServices;

public static class CloudinaryExtensions
{
    public static void AddCloudinaryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudinaryOptions = new CloudinaryOptions();
        configuration.GetSection(nameof(CloudinaryOptions)).Bind(cloudinaryOptions);

        services.AddSingleton<Cloudinary>((_) => new Cloudinary(
            new Account(
                cloudinaryOptions.CloudName,
                cloudinaryOptions.ApiKey,
                cloudinaryOptions.ApiSecret
            )
        ));

        services.AddSingleton<ICloudinaryService, CloudinaryServices>();
    }
}