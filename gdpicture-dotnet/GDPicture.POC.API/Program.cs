using GDPicture.POC.API;
using GdPicture14.WEB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddControllers();

builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
DocuViewareLicensing.RegisterKEY("0402583831552455551491240");
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

app.UseHttpsRedirection();

app.UseCors(options => options.
    AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials());

app.UseAuthorization();

app.MapControllers();

app.Run();
