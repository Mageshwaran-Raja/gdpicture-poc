using Azure.Storage.Blobs;
using GDPicture.POC.Application.Contracts.Azure;
using GDPicture.POC.Application.Contracts.GDPicture;
using Microsoft.Extensions.Configuration;

namespace GDPicture.POC.Application.AzureService
{
    public class AzureBlobStorage : IAzureBlobStorage
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IGDPictureService _gdPictureService;
        private readonly string _containerName;

        public AzureBlobStorage(IConfiguration configuration, BlobServiceClient blobServiceClient,
            IGDPictureService gdPictureService)
        {
            _configuration = configuration;
            _blobServiceClient = blobServiceClient;
            _gdPictureService = gdPictureService;
            _containerName = _configuration["ContainerName"];
        }
        public async Task DeleteAsync(BlobClient blobClient)
        {
            if (blobClient != null)
                await blobClient.DeleteAsync();
        }

        public async Task ProcessFileToBlob(string fileGuid, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            BlobClient blob = containerClient.GetBlobClient(fileName);

            if (await blob.ExistsAsync())
            {
                var property = blob.GetProperties().Value.Metadata;
                var fileId = property.Where(x => x.Key == "FileId").First().Value;
                var contentType = property.Where(x => x.Key == "ContentType").First().Value;

                if (fileId == fileGuid && contentType != "pdf")
                {
                    using (var stream = _gdPictureService.ProcessFileToBlob(blob.OpenRead(), fileId))
                    {
                        await UploadAsync(stream, fileId, "output.pdf", "pdf");
                    }
                    await DeleteAsync(blob);
                }
                File.Delete("output.pdf");
            }
        }

        public async Task UploadAsync(Stream fileStream, string fileGuid, string fileName, string contentType)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blob = containerClient.GetBlobClient(fileName);
            await blob.UploadAsync(fileStream, true);

            var metadata = GenerateMetaData(fileGuid, contentType);
            await blob.SetMetadataAsync(metadata);
        }

        private IDictionary<string, string> GenerateMetaData(string fileId, string contentType)
        {
            var metadata = new Dictionary<string, string>
            {
                { "FileId", fileId },
                { "ContentType", contentType },
            };
            return metadata;
        }
    }
}
