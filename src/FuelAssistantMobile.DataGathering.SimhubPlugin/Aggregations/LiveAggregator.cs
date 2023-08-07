using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations
{
    public sealed class LiveAggregator : ILiveAggregator
    {
        private readonly List<object> _aggregatedData = new List<object>();

        public void Add(object data)
        {
            _aggregatedData.Add(data);
        }

        public void Clear()
        {
            _aggregatedData.Clear();

        }

        public IEnumerable<object> ToList()
        {
            return _aggregatedData.ToList();
        }
    }
}
