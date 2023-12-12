using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GDPicture.POC.ServiceBusTrigger
{
    public class Function1
    {
        private readonly HttpClient _httpClient;
        public Function1(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [FunctionName("Function1")]
        public async Task RunAsync(
            [ServiceBusTrigger("jdlearningqueue", Connection = "ServiceBusConnectionString")]
            string myQueueItem, 
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            try 
            {
                var dictionObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(myQueueItem);

                if (dictionObj.TryGetValue("guid", out string guid) && dictionObj.TryGetValue("filename", out string filename))
                {
                    log.LogInformation($"C# ServiceBus queue trigger function processed message: GUID: {guid}");

                    var functionAppUrl = $"https://localhost:7049/api/File?id={guid}&filename={filename}";

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
                else
                {
                    log.LogWarning($"C# ServiceBus queue trigger function processed message: No 'guid' property found");
                }

            }
            catch (Exception ex) 
            {
                log.LogError(ex.Message);
            }
        }
    }
}
