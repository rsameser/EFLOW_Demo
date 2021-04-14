using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EFLOW_LVA
{
    public static class EdgeInterop
    {
        public static string iotHubConnectionString;
        public static string certificatePath;
        public static string gatewayHost;
        public static DeviceClient deviceClient;

        public static void SubscribeToEdgeMessages()
        {
            try
            {
                // Start by installing the certificate
                InstallCertificate();

                // Connect and subscribe to incoming messages
                System.Diagnostics.Debug.WriteLine("Starting EFLOW+LVA Main Program");
                if (!String.IsNullOrWhiteSpace(EdgeInterop.certificatePath) && !String.IsNullOrWhiteSpace(EdgeInterop.iotHubConnectionString) && !String.IsNullOrWhiteSpace(EdgeInterop.gatewayHost))
                {
                    string deviceConnectionString = $"{EdgeInterop.iotHubConnectionString};GatewayHostName={EdgeInterop.gatewayHost}";
                    deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);
                    deviceClient.SetMethodHandlerAsync("LeafDeviceDirectMethod", LeafDeviceMethodCallback, null).Wait();
                }
                else
                {
                    throw new Exception("Edge Interop parameters cannot be empty");
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to connect");
            }
           
        }

        public static void Unsubscribe()
        {
            deviceClient.CloseAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Looks the certificate at certificatePath and attempts to install the certificate
        /// </summary>
        private static void InstallCertificate()
        {
            if (!String.IsNullOrWhiteSpace(EdgeInterop.certificatePath))
            {
                CertificateManager.InstallCACert(EdgeInterop.certificatePath);
            }
        }

        /// <summary>
        /// This method will handle the incoming messages from the EFLOW Edge Module
        /// </summary>
        private static Task<MethodResponse> LeafDeviceMethodCallback(MethodRequest methodRequest, object userContext)
        {
            try
            {
                if (methodRequest.Data != null)
                {
                    string data = Encoding.UTF8.GetString(methodRequest.Data);
                    System.Diagnostics.Debug.WriteLine($"Message from Edge: {data}");

                    if (!String.IsNullOrEmpty(data))
                    {
                        try
                        {
                            Root inf_data = JsonConvert.DeserializeObject<Root>(data);

                            if(inf_data != null && inf_data.inferences.Count > 0)
                            {
                                foreach(var inf in inf_data.inferences)
                                {
                                    Program.sample.RefreshEdgeLogs(Newtonsoft.Json.JsonConvert.SerializeObject(inf.entity));
                                    Program.sample.RefreshEdgeBBOX(inf.entity.box.l, inf.entity.box.t, inf.entity.box.w, inf.entity.box.h, inf.entity.tag.value, inf.entity.tag.confidence);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                      
                    }

                    string jString = JsonConvert.SerializeObject("Success");
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jString), 200));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Message from Edge: Empty");
                    string jString = JsonConvert.SerializeObject("Empty");
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jString), 400));
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("Error de parsing"), 400));
            }
          
        }

        /// <summary>
        /// This method will send message to edge gateway/iothub.
        /// </summary>
        private static async Task SendMessage(string message)
        {
            Console.WriteLine($"Device says: {message}");
            using (var eventMessage = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(message)))
            {
                // Set the content type and encoding so the IoT Hub knows to treat the message body as JSON
                eventMessage.ContentEncoding = "utf-8";
                eventMessage.ContentType = "application/json";
                await deviceClient.SendEventAsync(eventMessage);
            }
        }

        private struct PeopleCount{
            public int count { get; set; }
        }
    
    }
}


public class Tag
{
    public string value { get; set; }
    public double confidence { get; set; }
}

public class Box
{
    public double l { get; set; }
    public double t { get; set; }
    public double w { get; set; }
    public double h { get; set; }
}

public class Entity
{
    public Tag tag { get; set; }
    public Box box { get; set; }
}

public class Inference
{
    public string type { get; set; }
    public string subtype { get; set; }
    public Entity entity { get; set; }
}

public class Root
{
    public long timestamp { get; set; }
    public List<Inference> inferences { get; set; }
}
