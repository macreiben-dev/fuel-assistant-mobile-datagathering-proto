using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Timers;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin
{
    [PluginDescription("Broadcast data to a remote API to work on race strategy.")]
    [PluginAuthor("Christian \"MacReiben\" Finel")]
    [PluginName("Fam Data Plugin")]
    public sealed class WebApiForwarder : IDataPlugin
    {
        private const int Frequency = 10; // 10Hz
        private HttpClient _httpClient;
        private Timer _postTimer;
        // THOUGHT: make this configurable.
        private readonly string webApiUrl = "https://example.com/api/data"; // Replace with your WebAPI URL

        public WebApiForwarder(PluginManager pluginManager, HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _postTimer = new Timer(1000 / Frequency); // Interval in milliseconds for 10Hz (1000ms / 10Hz = 100ms)
            _postTimer.Elapsed += PostData;
            _httpClient = httpClient;
        }

        public PluginManager PluginManager { set => throw new NotImplementedException(); }

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            throw new NotImplementedException();
        }

        public void End(PluginManager pluginManager)
        {
            throw new NotImplementedException();
        }

        public void Init(PluginManager pluginManager)
        {
            _postTimer.Start();
        }

        private async void PostData(object sender, ElapsedEventArgs e)
        {
            //if (CurrentContext == UpdateContext.Simulation)
            //{
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
                    var response = await _httpClient.PostAsync(webApiUrl, content);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        // Handle error cases here
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions here
                }
            //}
        }

        // THOUGHT: to be moved in repository
        private object GetRPM()
        {
            throw new NotImplementedException();
        }

        // THOUGHT: to be moved in repository
        private object GetSpeed()
        {
            throw new NotImplementedException();
        }
    }
}
