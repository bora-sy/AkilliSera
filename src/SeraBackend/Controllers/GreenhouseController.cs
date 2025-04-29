using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeraBackend.Configurations;
using SeraBackend.Greenhouse;

namespace SeraBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreenhouseController(ILogger<GreenhouseController> _logger, IOptions<GreenhouseConfiguration> _config, GreenhouseService _greenhouse) : ControllerBase
    {
        private readonly ILogger<GreenhouseController> _logger;


        [HttpPost("greenhousedata")]
        public IActionResult GreenhouseData(int NodeID, string humidValsStr)
        {
            if (NodeID < 0 || NodeID >= _config.Value.NodeCount) return BadRequest("Incorrect node ID");

            const char humidValSplitter = '-';

            string[] pairs = humidValsStr.Split(humidValSplitter);

            int[] humidVals = new int[pairs.Length];

            for(int i = 0; i < pairs.Length; i++)
            {
                if (!int.TryParse(pairs[i], out int humid)) return BadRequest("Malformed humidity values");
                else humidVals[i] = humid;
            }

            _greenhouse.OnNodeData(NodeID, humidVals);

            return Ok();
            
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(_greenhouse.GetNodes());
        }
    }
}
