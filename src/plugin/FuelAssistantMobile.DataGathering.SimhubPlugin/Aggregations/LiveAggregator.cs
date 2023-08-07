using System.Collections.Generic;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations
{
    public sealed class LiveAggregator : ILiveAggregator
    {
        private string _sessionTimeLeft = string.Empty;
        private bool _dirty = false;

        public bool IsDirty => _dirty;

        public void AddSessionTimeLeft(object sessionTimeLeft)
        {
            string trimmedSessionTimeLeft = sessionTimeLeft
                .ToString()
                .Substring(0, 8);

            if (_sessionTimeLeft != trimmedSessionTimeLeft)
            {
                _sessionTimeLeft = trimmedSessionTimeLeft;
                
                SetDirty();
            }
        }

        public void Clear()
        {
            SetClean();
            _sessionTimeLeft = string.Empty.ToString();
        }

        public Data AsData()
        {
            return new Data
            {
                SessionTimeLeft = _sessionTimeLeft
            };
        }

        private void SetDirty()
        {
            _dirty = true;
        }

        private void SetClean()
        {
            _dirty = true;
        }
    }
}
