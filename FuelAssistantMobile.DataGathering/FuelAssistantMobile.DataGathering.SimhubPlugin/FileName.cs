namespace FuelAssistantMobile.DataGathering.SimhubPlugin
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using SimHub.Plugins;

    namespace WebAPIForwarderPlugin
    {
        public class WebAPIForwarder : Plugin, IDataPlugin
        {
            private const int Frequency = 10; // 10Hz
            private readonly HttpClient httpClient;
            private readonly string webApiUrl = "https://example.com/api/data"; // Replace with your WebAPI URL

            private System.Timers.Timer postTimer;

            public WebAPIForwarder()
            {
                httpClient = new HttpClient();
                postTimer = new System.Timers.Timer(1000 / Frequency); // Interval in milliseconds for 10Hz (1000ms / 10Hz = 100ms)
                postTimer.Elapsed += PostData;
            }

            public override string Name => "WebAPI Forwarder";

            public void EndNewSession()
            {
                // Optional: Add cleanup logic when the session ends
            }

            public override void Init(PluginManager pluginManager)
            {
                // Initialize your plugin here
                postTimer.Start();
            }

            public async void DataUpdate(object sender, EventArgs e)
            {
                // This method will still be called whenever the data updates,
                // but we'll handle the posting at the desired frequency in PostData method.
            }

            private async void PostData(object sender, System.Timers.ElapsedEventArgs e)
            {
                if (CurrentContext == UpdateContext.Simulation)
                {
                    try
                    {
                        // Replace the following lines with your own logic to get the data you want to send
                        var dataToSend = new
                        {
                            speed = GetSpeed(),
                            rpm = GetRPM(),
                            // Add other data fields here
                        };

                        // Convert the data to JSON
                        var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        // Post the data to the WebAPI
                        var response = await httpClient.PostAsync(webApiUrl, content);

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            // Handle error cases here
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions here
                    }
                }
            }

            private double GetSpeed()
            {
                // Replace with your logic to get the current speed from SimHub
                // Example: return SimHubAPI.Instance.TelemetryInfo.SpeedKmh;
                return 0;
            }

            private int GetRPM()
            {
                // Replace with your logic to get the current RPM from SimHub
                // Example: return SimHubAPI.Instance.TelemetryInfo.Rpms;
                return 0;
            }
        }
    }

}
