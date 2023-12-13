using Azure.Storage.Blobs;

namespace GDPicture.POC.Application.Contracts.Azure
{
    public interface IAzureBlobStorage
    {
        Task UploadAsync(Stream fileStream, string fileGuid, string fileName, string contentType); 
        Task DeleteAsync(BlobClient blobClient);
        Task ProcessFileToBlob(string fileGuid, string fileName);
    }
}
