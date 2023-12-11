namespace GDPicture.POC.API.Services
{
    public interface IAzureServiceBusQueue
    {
        Task<string> SendMessageAsync(IFormFile formFile);
    }
}