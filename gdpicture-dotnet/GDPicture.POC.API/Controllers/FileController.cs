using GDPicture.POC.API.Services;
using GDPicture.POC.API.SignalR;
using GDPicture.POC.Application.Contracts.Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GDPicture.POC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IAzureServiceBusQueue _azureServiceBusQueue;
        private readonly IAzureQueueService _azureQueueService;
        private readonly IAzureBlobStorage _azureBlobStorage;
        private readonly IAzureBlobStorageService _azureBlobStorageService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public FileController(IAzureServiceBusQueue azureServiceBusQueue, IAzureBlobStorageService azureBlobStorageService,
            IHubContext<NotificationHub> hubContext, IAzureQueueService azureQueueService, IAzureBlobStorage azureBlobStorage)
        {
            _azureServiceBusQueue = azureServiceBusQueue;
            _azureBlobStorageService = azureBlobStorageService;
            _azureQueueService = azureQueueService;
            _hubContext = hubContext;
            _azureBlobStorage = azureBlobStorage;
        }

        [HttpGet]
        public async Task<IActionResult> GetFile(string id, string filename)
        {
            var message = await _azureBlobStorageService.GetFileAsync(id, filename);
            await _hubContext.Clients.All.SendAsync("ConversionCompleted", message);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            var id = await _azureServiceBusQueue.SendMessageAsync(formFile);

            return Accepted(
                new {
                    Id = id,
                    Message = "In Progress",
                    FileName = formFile.FileName
                }
            );
        }

        [HttpGet("ProcessFile")]
        public async Task<IActionResult> ProcessFile(string id, string filename)
        {
            await _azureBlobStorage.ProcessFileToBlob(id, filename);
            //await _hubContext.Clients.All.SendAsync("ConversionCompleted", message);
            return Ok();
        }

        [HttpPost("UploadAsync")]
        public async Task<IActionResult> UploadAsync(IFormFile formFile)
        {
            var id = string.Empty;  
            using (var stream = formFile.OpenReadStream())
            {
                id = await _azureQueueService.SendMessageAsync(stream, formFile.FileName, formFile.ContentType);
            }
            return Accepted(
                new
                {
                    Id = id,
                    Message = "In Progress",
                    FileName = formFile.FileName
                }
            );
        }
    }
}