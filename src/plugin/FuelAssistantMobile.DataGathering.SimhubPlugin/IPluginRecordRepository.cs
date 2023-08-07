namespace FuelAssistantMobile.DataGathering.SimhubPlugin
{
    public interface IPluginRecordRepository
    {
        bool IsGameRunning { get; }

        string SessionTimeLeft { get; }
    }
}