
using System.Text;
using System.Text.Json;
using Microsoft.Azure.ServiceBus;

namespace GDPicture.POC.API.Services
{
    public class AzureServiceBusQueue : IAzureServiceBusQueue
    {
        private readonly IConfiguration _configuration;
        private readonly IAzureBlobStorageService _azureBlobStorageService;
        public AzureServiceBusQueue(IConfiguration configuration, IAzureBlobStorageService azureBlobStorageService)
        {
            _configuration = configuration;
            _azureBlobStorageService = azureBlobStorageService;
        }
        public async Task<string> SendMessageAsync(IFormFile formFile)
        {
            string guid = Guid.NewGuid().ToString();

            IQueueClient client = new QueueClient(_configuration["AzureServiceBusConnectionString"], _configuration["QueueName"]);

            var message = GenerateMessage(guid, formFile.FileName);

            await _azureBlobStorageService.UploadAsync(formFile, guid);
            await client.SendAsync(message);

            return guid;
        }

        private Message GenerateMessage(string guid, string fileName)
        {
            string messageBody = JsonSerializer.Serialize(new { guid, filename = fileName });

            var message = new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                MessageId = guid,
                ContentType = "application/json"
            };

            return message;
        }
    }
}