using SimHub.Plugins;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin.PluginManagerWrappers
{
    public sealed class PluginRecordFactory : IPluginRecordFactory
    {
        public IPluginRecordRepository GetInstance(PluginManager pluginManager)
        {
            return new PluginManagerWrapper(new PluginManagerAdapter(pluginManager));
        }
    }
}
