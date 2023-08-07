using SimHub.Plugins;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin
{
    public interface IPluginRecordFactory
    {
        IPluginRecordRepository GetInstance(PluginManager pluginManager);
    }
}