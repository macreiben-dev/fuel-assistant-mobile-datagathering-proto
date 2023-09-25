using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace FuelAssistant.Vortex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly Gauge _gauge;
        private readonly DateTime _dummyNow;

        readonly string[] labels = new[] { "PilotName" };

        public TelemetryController()
        {
            var config = new GaugeConfiguration();

            _gauge = Metrics.CreateGauge("FuelAssistant_laptime_seconds", "Car laptimes in seconds.", config);

            _dummyNow = DateTime.UtcNow;
        }

        [HttpPost]
        public void Post(TelemetryModel telemetry)
        {
            TelemetryModel sample = new TelemetryModel();

            var diff = DateTime.UtcNow - _dummyNow;

            sample.PilotName = telemetry.PilotName;
            sample.LapTime = telemetry.LapTime;

            _gauge.Set(Convert.ToDouble(sample.LapTime));
        }
    }
}
