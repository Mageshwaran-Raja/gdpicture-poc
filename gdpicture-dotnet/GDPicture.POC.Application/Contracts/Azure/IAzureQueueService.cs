namespace GDPicture.POC.Application.Contracts.Azure
{
    public interface IAzureQueueService
    {
        Task<string> SendMessageAsync(Stream formFile, string fileName, string contentType);
    }
}
