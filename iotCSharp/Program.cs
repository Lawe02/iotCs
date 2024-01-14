using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using TransportType = Microsoft.Azure.Devices.Client.TransportType;

namespace iotCharp;
class Program
{
    private static bool continueSendingData = true;
    private static int dataSendingInterval = 5000; // default interval in milliseconds

    public static int Interval
    {
        get { return dataSendingInterval; }
        set { dataSendingInterval = value * 1000; } // convert seconds to milliseconds
    }

    static async Task Main()
    {
        // Initialize Azure IoT Hub connection
        var deviceId = "ducklin";
        var connectionString = $"HostName=laweit.azure-devices.net;DeviceId=ducklin;SharedAccessKey=e/WqUaNwqIpPMzIR9zRbuL8PXOo6zV6ELAIoTLH1w54=";
        var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);

        // Check if the device is registered in Azure IoT Hub, register if not
        var isRegistered = await IsDeviceRegisteredAsync(deviceClient, deviceId);
        if (!isRegistered)
        {
            await RegisterDeviceAsync(deviceClient, deviceId);
            // Save the connection locally if needed
        }

        // Handle Direct Methods
        await ReceiveDirectMethodAsync(deviceClient);

        // Simulate data
        while (true)
        {
            if (continueSendingData)
            {
                var data = new
                {
                    Temperature = 25.5,
                    Humidity = 60.0,
                    Status = "Operational"
                };

                // Send data to Azure IoT Hub
                await SendDataToIoTHubAsync(deviceClient, data);
            }
            else
            {
                Console.WriteLine("Data sending stopped.");
            }
            // Delay for 5 seconds
            await Task.Delay(Interval);
        }


    }

    // Check if the device is registered in Azure IoT Hub
    static async Task<bool> IsDeviceRegisteredAsync(DeviceClient deviceClient, string deviceId)
    {
        var twin = await deviceClient.GetTwinAsync();
        return !string.IsNullOrEmpty(twin.DeviceId);
    }

    // Register the device in Azure IoT Hub
    static async Task RegisterDeviceAsync(DeviceClient deviceClient, string deviceId)
    {
        var twin = new Twin();
        twin.DeviceId = deviceId;
        await deviceClient.UpdateReportedPropertiesAsync(twin.Tags);
    }

    // Send data to Azure IoT Hub
    static async Task SendDataToIoTHubAsync(DeviceClient deviceClient, object data)
    {
        var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
        await deviceClient.SendEventAsync(message);
    }


    // Receive and handle Direct Methods
    static async Task ReceiveDirectMethodAsync(DeviceClient deviceClient)
    {
        await deviceClient.SetMethodHandlerAsync("SetDataInterval", SetDataIntervalMethod, null);
        await deviceClient.SetMethodHandlerAsync("StopSendingData", StopSendingDataMethod, null);
        await deviceClient.SetMethodHandlerAsync("StartSendingData", StartSendingDataMethod, null);
    }

    static Task<MethodResponse> SetDataIntervalMethod(MethodRequest methodRequest, object userContext)
    {
        // Parse the requested interval from the payload
        int newInterval;
        if (int.TryParse(methodRequest.DataAsJson, out newInterval))
        {
            // Set the new interval
            Interval = newInterval;

            // Respond with success message
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes($"Interval set to {newInterval} seconds"), 200));
        }
        else
        {
            // Respond with an error message if the interval is not a valid integer
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("Invalid interval format"), 400));
        }
    }

    static Task<MethodResponse> StopSendingDataMethod(MethodRequest methodRequest, object userContext)
    {
        continueSendingData = false;

        return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("DataSendingStopped"), 200));
    }

    static Task<MethodResponse> StartSendingDataMethod(MethodRequest methodRequest, object userContext)
    {
        continueSendingData = true;

        return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("DataSendingStopped"), 200));
    }
}
