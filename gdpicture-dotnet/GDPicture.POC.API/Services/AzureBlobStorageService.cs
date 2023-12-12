using Azure.Storage.Blobs;
using GDPicture.POC.API.SignalR;
using GdPicture14;

namespace GDPicture.POC.API.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorageService(IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            _configuration = configuration;
            _blobServiceClient = blobServiceClient;
            _containerName = _configuration["ContainerName"];
        }

        public async Task<NotificationMessage> GetFileAsync(string guidId, string filename)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            BlobClient blob = containerClient.GetBlobClient(filename);
            NotificationMessage message = new();


            if (await blob.ExistsAsync())
            {
                var property = blob.GetProperties().Value.Metadata;
                var fileGuid = property.Where(x => x.Key == "FileId").First().Value;
                var contentType = property.Where(x => x.Key == "ContentType").First().Value;

                if (fileGuid == guidId && contentType != "pdf")
                {
                    using (GdPictureDocumentConverter oConverter = new GdPictureDocumentConverter())
                    {
                        GdPictureStatus status = oConverter.LoadFromStream(blob.OpenRead(), GdPicture14.DocumentFormat.DocumentFormatDOCX);

                        if (status == GdPictureStatus.OK)
                        {
                            //Select the conformance of the resulting PDF document.
                            status = oConverter.SaveAsPDF("output.pdf", PdfConformance.PDF_A_1a);
                            if (status == GdPictureStatus.OK)
                            {

                                using Stream uploadFileStream = File.OpenRead("output.pdf");

                                BlobClient pdfConvertedBlob = containerClient.GetBlobClient("output.pdf");

                                var metadata = GenerateMetaData(guidId, "pdf");
                                await pdfConvertedBlob.UploadAsync(uploadFileStream, true);

                                await pdfConvertedBlob.SetMetadataAsync(metadata);

                                uploadFileStream.Close();

                                message = new NotificationMessage
                                {
                                    Id = guidId,
                                    Message = "File is Converted to pdf",
                                    FileURL = pdfConvertedBlob.Uri.ToString(),
                                };

                                await blob.DeleteAsync();
                            }
                            else
                            {
                                throw new Exception("Cannot Convert to pdf");
                            }
                        }
                        else
                        {
                            throw new Exception("Cannot Convert to pdf");
                        }
                    }
                    File.Delete("output.pdf");
                }
                Console.WriteLine("File Exists");
            }
            return message;
        }

        public async Task UploadAsync(IFormFile file, string reqId)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            using (var uploadStream = file.OpenReadStream())
            {
                var blob = containerClient.GetBlobClient(file.FileName);
                await blob.UploadAsync(uploadStream, true);

                var metadata = GenerateMetaData(reqId, file.ContentType);
                await blob.SetMetadataAsync(metadata);
            }
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

        private async Task ConvertToPDF(Stream fileStream, string fileName)
        {
            using (GdPictureDocumentConverter oConverter = new GdPictureDocumentConverter())
            {
                GdPictureStatus status = oConverter.LoadFromStream(fileStream, GdPicture14.DocumentFormat.DocumentFormatDOCX);

                if (status == GdPictureStatus.OK)
                {
                    status = oConverter.SaveAsPDF("output.pdf", PdfConformance.PDF_A_1a);
                    if (status == GdPictureStatus.OK)
                    {
                        using Stream uploadFileStream = File.OpenRead("output.pdf");

                        await UploadFileToBlobStorage(uploadFileStream, "output.pdf");

                        uploadFileStream.Close();
                    }
                    else
                    {
                        throw new Exception("Cannot Convert to pdf");
                    }
                }
                else
                {
                    throw new Exception("Cannot Convert to pdf");
                }
            }
        }
        private async Task UploadFileToBlobStorage(Stream fileStream, string fileName) 
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            BlobClient blob = containerClient.GetBlobClient(fileName);

            await blob.UploadAsync(fileStream, true);
        } 
    }
}