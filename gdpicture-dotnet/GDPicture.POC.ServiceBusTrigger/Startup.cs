using GDPicture.POC.API.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.ComponentModel;

[assembly: FunctionsStartup(typeof(GDPicture.POC.ServiceBusTrigger.Startup))]  
namespace GDPicture.POC.ServiceBusTrigger
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
        }
    }
}
