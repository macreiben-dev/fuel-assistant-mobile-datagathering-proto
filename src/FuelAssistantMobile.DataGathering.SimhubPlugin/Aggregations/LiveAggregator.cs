using System.Collections.Generic;
using System.Linq;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations
{
    public sealed class LiveAggregator : ILiveAggregator
    {
        private readonly List<object> _aggregatedData = new List<object>();
        private string _sessionTimeLeft = string.Empty;
        private bool _dirty = false;

        public bool IsDirty => _dirty;

        public void AddSessionTimeLeft(string sessionTimeLeft)
        {
            if(_sessionTimeLeft != sessionTimeLeft) {
                _sessionTimeLeft = sessionTimeLeft;

                _dirty = true;
            }
        }

        public void Add(object data)
        {
            _aggregatedData.Add(data);

            _dirty = true;
        }

        public void Clear()
        {
            _dirty = false;
            _aggregatedData.Clear();
            _sessionTimeLeft = string.Empty;
        }

        public object AsData()
        {
            return new
            {
                SessionTimeLeft = _sessionTimeLeft
            };
        }
    }
}
