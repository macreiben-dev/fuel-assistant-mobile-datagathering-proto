using System.Collections.Generic;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations
{
    public interface ILiveAggregator
    {
        bool IsDirty { get; }

        void Add(object data);

        void AddSessionTimeLeft(object sessionTimeLeft);

        void Clear();

        Data AsData();
    }
}