using FuelAssistantMobile.DataGathering.SimhubPlugin;
using NFluent;
using Xunit;

namespace Fam.DataGathering.ShPlugin
{
    public class WebApiForwarderTest
    {
        [Fact]
        public void Should_build_webapi_forwarder()
        {
            Check.ThatCode(() => { new WebApiForwarder(); });
        }
    }
}
