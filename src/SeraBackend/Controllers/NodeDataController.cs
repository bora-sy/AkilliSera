using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeraBackend.Configurations;
using SeraBackend.Greenhouse;

namespace SeraBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NodeDataController(ILogger<NodeDataController> _logger, IOptions<GreenhouseConfiguration> _config, GreenhouseService _greenhouse) : ControllerBase
    {
        private readonly ILogger<NodeDataController> _logger;


        [HttpPost("nodedata")]
        public IActionResult NodeData(int NodeID, int humidVal)
        {
            if (NodeID < 0 || NodeID >= _config.Value.NodeCount) return BadRequest("Incorrect node ID");

            _greenhouse.OnNodeData(NodeID, humidVal);

            return Ok();
            
        }
    }
}
