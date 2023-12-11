using Azure.Storage.Blobs;
using GDPicture.POC.API.SignalR;
using GdPicture14;

namespace GDPicture.POC.API.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["StorageConnectionAppSetting"];
            _containerName = _configuration["ContainerName"];
        }

        public async Task<NotificationMessage> GetFileAsync(string guidId, string filename)
        {
            var serviceClient = new BlobServiceClient(_connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(_containerName);

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

                                IDictionary<string, string> metadata = new Dictionary<string, string>();
                                await pdfConvertedBlob.UploadAsync(uploadFileStream, true);

                                metadata.Add("fileId", guidId);
                                metadata.Add("fileType", "pdf");
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
            var serviceClient = new BlobServiceClient(_connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(_containerName);

            using Stream uploadFileStream = file.OpenReadStream();

            BlobClient blob = containerClient.GetBlobClient(file.FileName);

            await blob.UploadAsync(uploadFileStream, true);

            var metadata = GenerateMetaData(reqId, file.ContentType);

            await blob.SetMetadataAsync(metadata);
            uploadFileStream.Close();
        }

        private IDictionary<string, string> GenerateMetaData(string fileId, string contentType)
        {
            var metadata = new Dictionary<string, string>();

            metadata.Add("FileId", fileId);
            metadata.Add("ContentType", contentType);

            return metadata;
        }

        private Stream ConvertToPDF(Stream fileStream, string fileName)
        {

            LicenseManager licenseManager = new LicenseManager();
            licenseManager.RegisterKEY("0402583831552455551491240");

            using (GdPictureDocumentConverter oConverter = new GdPictureDocumentConverter())
            //  using (GdPicturePDF oConverter = new GdPicturePDF())
            {
                //Select your source document and its document format.
                GdPictureStatus status = oConverter.LoadFromStream(fileStream, GdPicture14.DocumentFormat.DocumentFormatDOCX);

                //oConverter.LoadFromFile("input.docx", GdPicture14.DocumentFormat.DocumentFormatDOCX);
                if (status == GdPictureStatus.OK)
                {
                    //Select the conformance of the resulting PDF document.
                    status = oConverter.SaveAsPDF("output.pdf", PdfConformance.PDF_A_1a);
                    if (status == GdPictureStatus.OK)
                    {

                        using Stream uploadFileStream = File.OpenRead("output.pdf");

                        return uploadFileStream;
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
    }
}