using System.Collections.Generic;

namespace FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations
{
    public interface ILiveAggregator
    {
        bool IsDirty { get; }

        void AddSessionTimeLeft(object sessionTimeLeft);

        void Clear();

        Data AsData();
    }
}