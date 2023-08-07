using SimHub.Plugins;
using System;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin
{
    public sealed class PluginManagerWrapper : IPluginRecordRepository
    {
        private readonly bool _isGameRunning;
        private readonly string _sessionTimeLeft;

        public PluginManagerWrapper(PluginManager pluginManager)
        {
            _isGameRunning = ToBoolean("DataCorePlugin.GameRunning", pluginManager);

            _sessionTimeLeft = ToString("DataCorePlugin.GameData.SessionTimeLeft", pluginManager);
        }

        // ==================================================

        public bool IsGameRunning => _isGameRunning;

        public string SessionTimeLeft => _sessionTimeLeft;

        // ==================================================

        private string ToString(string key, PluginManager pluginManager)
        {
            var data = pluginManager.GetPropertyValue(key);

            return data.ToString();
        }

        private bool ToBoolean(string key, PluginManager pluginManager)
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
