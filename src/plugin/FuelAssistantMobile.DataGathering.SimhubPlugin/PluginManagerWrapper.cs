using SimHub.Plugins;
using System;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin
{
    public sealed class PluginManagerWrapper : IPluginRecordRepository
    {
        private PluginManager _pluginManager;

        public PluginManagerWrapper(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        // ==================================================

        public bool IsGameRunning => ToBoolean("DataCorePlugin.GameRunning");

        public string SessionTimeLeft => ToString("DataCorePlugin.GameData.SessionTimeLeft");

        // ==================================================

        private string ToString(string key)
        {
            var data = _pluginManager.GetPropertyValue(key);

            return data.ToString();
        }

        private bool ToBoolean(string key)
        {
            var data = _pluginManager.GetPropertyValue(key);

            if (data == null)
            {
                return false;
            }

            return Convert.ToBoolean(data);
        }
    }
}
