using Azure.Messaging.ServiceBus;
using GDPicture.POC.Application.Contracts.Azure;
using Microsoft.Extensions.Configuration;

namespace GDPicture.POC.Application.AzureService
{
    public class AzureQueueService : IAzureQueueService
    {
        private readonly IConfiguration _configuration;
        private readonly IAzureBlobStorage _azureBlobStorage;
        private readonly ServiceBusClient _serviceBusClient;
        public AzureQueueService(IConfiguration configuration, IAzureBlobStorage azureBlobStorage,
            ServiceBusClient serviceBusClient)
        {
            _configuration = configuration;
            _azureBlobStorage = azureBlobStorage;
            _serviceBusClient = serviceBusClient;
        }
        public async Task<string> SendMessageAsync(Stream formFile, string fileName, string contentType)
        {
            string guid = Guid.NewGuid().ToString();

            var message = GenerateMessage(guid, fileName);

            var senderClient = _serviceBusClient.CreateSender(_configuration["QueueName"]);

            await _azureBlobStorage.UploadAsync(formFile, guid, fileName, contentType);

            await senderClient.SendMessageAsync(message);

            return guid;
        }

        private ServiceBusMessage GenerateMessage(string guid, string fileName)
        {
            var serviceBusMessage = new ServiceBusMessage();

            serviceBusMessage.ApplicationProperties.Add("FileGuid", guid);
            serviceBusMessage.ApplicationProperties.Add("FileName", fileName);

            return serviceBusMessage;
        }
    }
}
