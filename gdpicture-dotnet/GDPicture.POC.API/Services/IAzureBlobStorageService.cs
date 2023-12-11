using GDPicture.POC.API.SignalR;

namespace GDPicture.POC.API.Services
{
    public interface IAzureBlobStorageService
    {
        Task UploadAsync(IFormFile file, string reqId);
        Task<NotificationMessage> GetFileAsync(string guidId, string filename);
    }
}