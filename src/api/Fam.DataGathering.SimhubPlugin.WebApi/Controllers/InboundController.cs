using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fam.DataGathering.SimhubPlugin.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InboundController
    {
        [HttpPost]
        public void Post([FromBody] object data)
        {
            Trace.WriteLine(data.ToString());
        }
    }
}
