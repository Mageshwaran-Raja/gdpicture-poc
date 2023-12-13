using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace GDPicture.POC.ServiceBusTrigger
{
    public class AsyncProcessingBackgroundWorker
    {
        private readonly HttpClient _httpClient;
        public AsyncProcessingBackgroundWorker(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [FunctionName("AsyncProcessingBackgroundWorker")]
        public async Task RunAsync([ServiceBusTrigger("jdlearningqueue", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {message}");

            var fileGuid = message.ApplicationProperties["FileGuid"];
            var fileName = message.ApplicationProperties["FileName"];

            try
            {
                var functionAppUrl = $"https://localhost:7049/api/File/ProcessFile?id={fileGuid}&filename={fileName}";

                var response = await _httpClient.GetAsync(functionAppUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    log.LogInformation("File Converted to PDF");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }
    }
}
