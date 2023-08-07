﻿using FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations;
using FuelAssistantMobile.DataGathering.SimhubPlugin.Logging;
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
    public sealed partial class WebApiForwarder : IDataPlugin
    {
        private const int Frequency = 10; // 10Hz
        private HttpClient _httpClient;
        private Timer _postTimer;
        private ILiveAggregator _liveAggregator;
        private ILogger _logger;

        // THOUGHT: make this configurable.
        private readonly string webApiUrl = "https://localhost:900/api/inbound"; // Replace with your WebAPI URL

        public WebApiForwarder(PluginManager pluginManager)
            : this(pluginManager, new SimhubLogger(), new LiveAggregator())
        {
            
        }

        public WebApiForwarder(PluginManager manager, ILogger logger, ILiveAggregator aggregator)
        {
            _httpClient = new HttpClient();

            _postTimer = new Timer(1000 / Frequency); // Interval in milliseconds for 10Hz (1000ms / 10Hz = 100ms)
            _postTimer.Elapsed += PostData;

            _liveAggregator = aggregator;

            _logger = logger;
        }

        public PluginManager PluginManager { set => throw new NotImplementedException(); }

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // THOUGHT: add call to service here to grab the data we want from plugin manager

            var GameDataSessionTimeLeft = pluginManager.GetPropertyValue("DataCorePlugin.GameData.SessionTimeLeft");

            _liveAggregator.Add(GameDataSessionTimeLeft);
        }

        public void End(PluginManager pluginManager)
        {
            _logger.Info("Data plugin closing ...");
            
            _liveAggregator.Clear();
            _postTimer.Stop();

            _logger.Info("Data plugin closing DONE!");
        }

        public void Init(PluginManager pluginManager)
        {
            _postTimer.Start();

            _liveAggregator.Clear(); // just in case ... 
        }

        // THOUGHT: this one should be moved in a dedicated class.
        private async void PostData(object sender, ElapsedEventArgs e)
        {
            // THOUGHT: check game status before doing anything. If it is not running. Then do nothing.

            try
            {
                // Replace the following lines with your own logic to get the data you want to send
                var dataToSend = new
                {
                    SessionTimeLeft = _liveAggregator.ToList()
                };

                // Convert the data to JSON
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Post the data to the WebAPI
                var response = await _httpClient.PostAsync(webApiUrl, content);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // Handle error cases here
                    _logger.Error($"API failed returned code is not OK - [{response.StatusCode}]");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Issue during posting.", ex);
            }
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
