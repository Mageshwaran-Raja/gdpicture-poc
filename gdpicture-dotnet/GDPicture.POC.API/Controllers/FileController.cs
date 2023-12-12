using GDPicture.POC.API.Services;
using GDPicture.POC.API.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GDPicture.POC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IAzureServiceBusQueue _azureServiceBusQueue;
        private readonly IAzureBlobStorageService _azureBlobStorageService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public FileController(IAzureServiceBusQueue azureServiceBusQueue, IAzureBlobStorageService azureBlobStorageService,
            IHubContext<NotificationHub> hubContext)
        {
            _azureServiceBusQueue = azureServiceBusQueue;
            _azureBlobStorageService = azureBlobStorageService;
            _hubContext = hubContext;
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
    }
}