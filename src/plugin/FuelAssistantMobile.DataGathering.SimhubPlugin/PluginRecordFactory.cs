using SimHub.Plugins;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin
{
    public sealed class PluginRecordFactory : IPluginRecordFactory
    {
        public IPluginRecordRepository GetInstance(PluginManager pluginManager)
        {
            return new PluginManagerWrapper(pluginManager);
        }
    }
}
