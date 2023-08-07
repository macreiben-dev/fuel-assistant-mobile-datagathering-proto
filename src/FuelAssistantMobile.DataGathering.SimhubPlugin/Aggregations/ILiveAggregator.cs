using System.Collections.Generic;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations
{
    public interface ILiveAggregator
    {
        void Add(object data);
        void Clear();

        IEnumerable<object> ToList();
    }
}