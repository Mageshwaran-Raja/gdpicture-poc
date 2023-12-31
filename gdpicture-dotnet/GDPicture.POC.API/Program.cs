using GDPicture.POC.API;
using GDPicture.POC.API.Services;
using GDPicture.POC.API.SignalR;
using GDPicture.POC.Application;
using GdPicture14;
using GdPicture14.WEB;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
builder.Services.AddScoped<IAzureServiceBusQueue, AzureServiceBusQueue>();

builder.Services.AddAzureClients(opt => {
    opt.AddBlobServiceClient(builder.Configuration.GetValue<string>("StorageConnectionAppSetting"));
    //opt.AddServiceBusClient(builder.Configuration.GetValue<string>("AzureServiceBusConnectionString"));
});

builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var gdPictureLicenseKey = builder.Configuration.GetConnectionString("GDPictureLicenseKey");

LicenseManager licenseManager = new LicenseManager();
licenseManager.RegisterKEY(gdPictureLicenseKey);

DocuViewareLicensing.RegisterKEY(gdPictureLicenseKey);
DocuViewareManager.SetupConfiguration(true, DocuViewareSessionStateMode.InProc,
    Path.Combine(Directory.GetCurrentDirectory(), "/cache"), "https://localhost:7049", "api/DocuVieware3");
DocuViewareEventsHandler.CustomAction += Global.CustomActionDispatcher;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();

app.UseRouting();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseCors(options => options.
    AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials());

app.UseEndpoints(endpoints =>
    endpoints.MapHub<NotificationHub>("/signalr/notificationHub")
);


app.MapControllers();

app.Run();
