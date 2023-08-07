using FuelAssistantMobile.DataGathering.SimhubPlugin.PluginManagerWrappers;
using NFluent;
using NSubstitute;
using Xunit;

namespace Fam.DataGathering.ShPlugin
{
    public class PluginManagerWrapperTest
    {
        private IPluginManagerAdapter _pluginManagerAdapter;

        public PluginManagerWrapperTest()
        {
            _pluginManagerAdapter = Substitute.For<IPluginManagerAdapter>();
        }

        private PluginManagerWrapper GetTarget()
        {
            return new PluginManagerWrapper(_pluginManagerAdapter);
        }

        [Fact]
        public void Should_map_is_gamingRunning()
        {
            _pluginManagerAdapter.GetPropertyValue("DataCorePlugin.GameRunning")
                .Returns(true);

            var target = GetTarget();   

            Check.That(target.IsGameRunning).IsTrue();
        }

        [Fact]
        public void Should_map_is_sessionTimeLeft()
        {
            _pluginManagerAdapter.GetPropertyValue("DataCorePlugin.GameData.SessionTimeLeft")
                .Returns("01:02:03.00000");

            var target = GetTarget();

            Check.That(target.SessionTimeLeft).IsEqualTo("01:02:03.00000");
        }
    }
}
