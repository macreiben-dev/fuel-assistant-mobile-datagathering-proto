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
        private readonly WebApiForwarderService _webApiForwarderService;
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
            _logger = logger;

            _pluginRecordFactory = pluginRecordFactory;

            _webApiForwarderService = new WebApiForwarderService(
                aggregator,
                dataRepository,
                logger);
        }

        // ===========================================================

        public PluginManager PluginManager { set => _pluginManager = value; }

        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // THOUGHT: add call to service here to grab the data we want from plugin manager
            IPluginRecordRepository pluginRecordRepository
                = _pluginRecordFactory.GetInstance(pluginManager);

            _webApiForwarderService.HandleDataUpdate(pluginRecordRepository);
        }

        public void End(PluginManager pluginManager)
        {
            _logger.Info("Data plugin closing ...");

            _webApiForwarderService.Stop();

            _logger.Info("Data plugin closing DONE!");
        }

        public void Init(PluginManager pluginManager)
        {
            _logger.Info("Starting Fam Data Gathering plugin ...");

            _webApiForwarderService.Start();

            _logger.Info("Starting Fam Data Gathering plugin DONE!");
        }

        // ===========================================================

    }
}
