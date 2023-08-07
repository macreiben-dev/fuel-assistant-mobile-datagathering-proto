using FuelAssistantMobile.DataGathering.SimhubPlugin.Aggregations;
using NFluent;
using System.Diagnostics;
using Xunit;

namespace Fam.DataGathering.ShPlugin.Aggregations
{
    public class LiveAggregatorTest
    {
        private LiveAggregator GetTarget()
        {
            return new LiveAggregator();
        }

        [Fact] 
        public void Should_trim_sessionLeft()
        {
            // ARRANGE
            var original = "00:56:39.1970000";

            var target = GetTarget();

            // ACT
            Stopwatch watch = Stopwatch.StartNew();

            target.AddSessionTimeLeft(original);

            watch.Stop();

            var actual = target.AsData();

            // ASSERT
            Check.That(actual.SessionTimeLeft).IsEqualTo("00:56:39");
            Check.That(watch.ElapsedMilliseconds).IsLessOrEqualThan(3);
        }
    }
}
