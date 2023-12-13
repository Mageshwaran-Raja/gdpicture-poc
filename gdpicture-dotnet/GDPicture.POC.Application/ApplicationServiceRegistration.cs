using GDPicture.POC.Application.AzureService;
using GDPicture.POC.Application.Contracts.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using GDPicture.POC.Application.Contracts.GDPicture;
namespace GDPicture.POC.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAzureQueueService, AzureQueueService>();
            services.AddScoped<IAzureBlobStorage, AzureBlobStorage>();
            services.AddScoped<IGDPictureService, GDPictureService.GDPictureService>();

            services.AddAzureClients(opt => {
                opt.AddBlobServiceClient(configuration.GetValue<string>("StorageConnectionAppSetting"));
                opt.AddServiceBusClient(configuration.GetValue<string>("AzureServiceBusConnectionString"));
            });

            return services;
        }
    }
}
