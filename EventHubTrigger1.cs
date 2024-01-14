using System;
using System.Text;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SuperFunction.Contexts;
using Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using vsFunction.Models;
using Microsoft.Azure.Cosmos.Core;

namespace Barm
{
    public class EventHubTrigger1
    {
        private readonly ILogger<EventHubTrigger1> _logger;
        private readonly CosmosContext context;

        public EventHubTrigger1(ILogger<EventHubTrigger1> logger, CosmosContext context)
        {
            this.context = context;
            _logger = logger;
        }

        [Function(nameof(Function))]
        public async Task Run([EventHubTrigger("iothub-ehub-laweit-25369970-1ce13bddaa", Connection = "IotHubEndpoint")] EventData[] events)
        {
            foreach (EventData @event in events)
            {
                var iotDataJson = Encoding.UTF8.GetString(@event.Body.ToArray());
                var iotData = JsonConvert.DeserializeObject<IoTData>(iotDataJson);
                
                var item = new Duck
                {
                    Id = Guid.NewGuid().ToString(),
                    dsfdsfggfdhfgh = iotData.Temperature.ToString(),
                    Test = iotData.Status.ToString(),
                };
                await context.Ober.AddAsync(item);

                await context.SaveChangesAsync();
            }
        }
    }
}
