using SimHub.Plugins;
using System;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin.PluginManagerWrappers
{
    public sealed class PluginManagerWrapper : IPluginRecordRepository
    {
        private readonly IPluginManagerAdapter _pluginManager;
        private readonly bool _isGameRunning;
        private readonly string _sessionTimeLeft;

        public PluginManagerWrapper(IPluginManagerAdapter pluginManager)
        {
            _pluginManager = pluginManager;
        }

        // ==================================================

        public bool IsGameRunning => 
            ToBoolean("DataCorePlugin.GameRunning", _pluginManager);

        public string SessionTimeLeft => 
            ToString("DataCorePlugin.GameData.SessionTimeLeft", _pluginManager);

        // ==================================================

        private string ToString(string key, IPluginManagerAdapter pluginManager)
        {
            var data = pluginManager.GetPropertyValue(key);

            return data.ToString();
        }

        private bool ToBoolean(string key, IPluginManagerAdapter pluginManager)
        {
            var data = pluginManager.GetPropertyValue(key);

            if (data == null)
            {
                return false;
            }

            return Convert.ToBoolean(data);
        }
    }
}
