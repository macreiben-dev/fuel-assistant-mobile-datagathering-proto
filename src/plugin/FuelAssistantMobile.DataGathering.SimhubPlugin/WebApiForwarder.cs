using FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations;
using FuelAssistantMobile.DataGathering.SimhubPlugin.Logging;
using FuelAssistantMobile.DataGathering.SimhubPlugin.Repositories;
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

        // THOUGHT: make this configurable.
        private const string WebApiUrl = "https://localhost:32786/Inbound";

        private HttpClient _httpClient;
        private Timer _postTimer;
        private Timer _autoReactivate;
        private ILiveAggregator _liveAggregator;
        private IStagingDataRepository _dataRepository;
        private readonly IStagingDataRepository dataRepository;
        private ILogger _logger;
        private PluginManager _pluginManager;

        private int _internalErrorCount = 0;
        private bool _notifiedStop = false;

        public WebApiForwarder()
            : this(
                  new SimhubLogger(),
                  new LiveAggregator(),
                  new FamRemoteRepository())
        {

        }

        public WebApiForwarder(
            ILogger logger,
            ILiveAggregator aggregator,
            IStagingDataRepository dataRepository)
        {
            _httpClient = new HttpClient();

            _postTimer = new Timer(1000 / Frequency); // Interval in milliseconds for 10Hz (1000ms / 10Hz = 100ms)
            _postTimer.Elapsed += PostData;

            _autoReactivate = new Timer(5000);
            _autoReactivate.Elapsed += AutoReactivate;

            _liveAggregator = aggregator;

            _dataRepository = dataRepository;
            _logger = logger;
        }

        public PluginManager PluginManager { set => _pluginManager = value; }

        // ===========================================================
        // ===========================================================

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // THOUGHT: add call to service here to grab the data we want from plugin manager

            if (_notifiedStop) return;

            var start = pluginManager.GetPropertyValue("DataCorePlugin.GameRunning");

            if (!Convert.ToBoolean(start))
            {
                return;
            }

            var gameDataSessionTimeLeft = pluginManager.GetPropertyValue("DataCorePlugin.GameData.SessionTimeLeft");

            _liveAggregator.AddSessionTimeLeft(gameDataSessionTimeLeft);
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

        // ===========================================================
        // ===========================================================

        private void Reset()
        {
            _liveAggregator.Clear();
            _postTimer.Start();

            _notifiedStop = false;

            _internalErrorCount = 0;
        }

        // THOUGHT: this one should be moved in a dedicated class.
        private async void PostData(object sender, ElapsedEventArgs e)
        {
            // THOUGHT: check game status before doing anything. If it is not running. Then do nothing.
            if (ShouldStopTimer())
            {
                try
                {
                    _postTimer.Stop();
                }
                catch { }

                _logger.Error("WebAPI post stoped.");
                _notifiedStop = true;
                return;
            }

            if (ShouldNotifyRetrying())
            {
                _logger.Warn($"Retrying to contact API - error count is [{_internalErrorCount}]");
            }

            if (!_liveAggregator.IsDirty)
            {
                return;
            }

            // Replace the following lines with your own logic to get the data you want to send
            var dataToSend = new
            {
                data = _liveAggregator.AsData()
            };

            try
            {
                await _dataRepository.SendAsync(dataToSend);
            }
            catch (ErrorWhenSendDataException ex)
            {
                _internalErrorCount++;

                _logger.Error($"Issue during posting [{ex.WebApiUrl}] - [{_internalErrorCount}] error count.");
                _logger.Error("Posted data is:");
                _logger.Error(ex.JsonData);
                _logger.Error($"Exception is:", ex);
            }
            catch (StatusCodeNotOkException ex)
            {
                _internalErrorCount++;

                _logger.Error($"Issue during posting [{ex.WebApiUrl}] - [{_internalErrorCount}] error count.");
                _logger.Error($"API failed returned code is not OK - [{ex.StatusCode}]");
            }
        }

        private bool ShouldStopTimer()
        {
            return _internalErrorCount >= 3;
        }

        private bool ShouldNotifyRetrying()
        {
            return _internalErrorCount > 0 && _internalErrorCount < 3;
        }

        private void AutoReactivate(object sender, ElapsedEventArgs e)
        {
            if (_notifiedStop && _internalErrorCount >= 3)
            {
                _logger.Info("Trying to restart plugin after errors ...");

                Reset();
            }
        }
    }
}
