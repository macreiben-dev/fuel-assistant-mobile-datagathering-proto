using FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations;
using FuelAssistantMobile.DataGathering.SimhubPlugin.Logging;
using FuelAssistantMobile.DataGathering.SimhubPlugin.PluginManagerWrappers;
using FuelAssistantMobile.DataGathering.SimhubPlugin.Repositories;
using GameReaderCommon;
using SimHub.Plugins;
using System.Timers;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin
{
    [PluginDescription("Broadcast data to a remote API to work on race strategy.")]
    [PluginAuthor("Christian \"MacReiben\" Finel")]
    [PluginName("Fam Data Plugin")]
    public sealed partial class WebApiForwarder : IDataPlugin
    {
        private const int Frequency = 10; // 10Hz

        private Timer _postTimer;
        private Timer _autoReactivate;
        
        private readonly ILiveAggregator _liveAggregator;
        private readonly IStagingDataRepository _dataRepository;
        private readonly IPluginRecordFactory _pluginRecordFactory;
        private readonly IStagingDataRepository dataRepository;
        private readonly ILogger _logger;

        private PluginManager _pluginManager;

        private int _internalErrorCount = 0;
        private bool _notifiedStop = false;
        private bool _firstLaunch = false;

        public WebApiForwarder()
            : this(
                  new SimhubLogger(),
                  new LiveAggregator(),
                  new FamRemoteRepository(),
                  new PluginRecordFactory())
        {

        }

        public WebApiForwarder(
            ILogger logger,
            ILiveAggregator aggregator,
            IStagingDataRepository dataRepository,
            IPluginRecordFactory pluginRecordFactory)
        {
            _postTimer = new Timer(1000 / Frequency); // Interval in milliseconds for 10Hz (1000ms / 10Hz = 100ms)
            _postTimer.Elapsed += PostData;

            _autoReactivate = new Timer(5000);
            _autoReactivate.Elapsed += AutoReactivate;

            _liveAggregator = aggregator;

            _dataRepository = dataRepository;
            _pluginRecordFactory = pluginRecordFactory;
            _logger = logger;
        }

        public PluginManager PluginManager { set => _pluginManager = value; }

        // ===========================================================

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // THOUGHT: add call to service here to grab the data we want from plugin manager
            IPluginRecordRepository pluginRecordRepository 
                = _pluginRecordFactory.GetInstance(pluginManager);
            
            var isGameRunning = pluginRecordRepository.IsGameRunning;

            if (!_firstLaunch && isGameRunning)
            {
                Start();

                UpdateAggregator(pluginRecordRepository);

                _firstLaunch = true;
            }

            if (!isGameRunning)
            {
                Stop();

                return;
            }

            if(isGameRunning)
            {
                Start();

                UpdateAggregator(pluginRecordRepository);

                return;
            }
        }

        public void End(PluginManager pluginManager)
        {
            _logger.Info("Data plugin closing ...");

            Stop();

            _logger.Info("Data plugin closing DONE!");
        }

        public void Init(PluginManager pluginManager)
        {
            _logger.Info("Starting Fam Data Gathering plugin ...");
            
            Start();

            _logger.Info("Starting Fam Data Gathering plugin DONE!");
        }

        // ===========================================================

        // THOUGHT: this one should be moved in a dedicated class.
        private async void PostData(object sender, ElapsedEventArgs e)
        {
            // THOUGHT: check game status before doing anything. If it is not running. Then do nothing.
            if (ShouldStopTimer())
            {
                try
                {
                    Stop();
                }
                catch { }

                _logger.Error("WebAPI post stoped.");

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

        private void UpdateAggregator(IPluginRecordRepository racingDataRepository)
        {
            var gameDataSessionTimeLeft = racingDataRepository.SessionTimeLeft;

            _liveAggregator.AddSessionTimeLeft(gameDataSessionTimeLeft);
        }

        private void Start()
        {
            if(_notifiedStop == false )
            {
                return;
            }

            _internalErrorCount = 0;
            _notifiedStop = false;
            _postTimer.Start();
            _liveAggregator.Clear();

            _logger.Info("Fam Data Gathering plugin STARTED");
        }

        private void Stop()
        {
            if(_notifiedStop == true)
            {
                return;
            }

            _liveAggregator.Clear();
            _postTimer.Stop();
            _internalErrorCount = 0;
            _notifiedStop = true;

            _logger.Info("Fam Data Gathering plugin STOPPED");
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

                Start();
            }
        }
    }
}
