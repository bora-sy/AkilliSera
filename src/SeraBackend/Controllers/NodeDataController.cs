using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeraBackend.Configurations;
using SeraBackend.Greenhouse;
using System.Net;

namespace SeraBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NodeDataController(ILogger<NodeDataController> _logger, IOptions<GreenhouseConfiguration> _config, GreenhouseService _greenhouse) : ControllerBase
    {
        private readonly ILogger<NodeDataController> _logger;


        [HttpPost("nodedata")]
        public IActionResult NodeData(int NodeID, int moistureval)
        {
            if (NodeID < 0 || NodeID >= _config.Value.NodeCount) return BadRequest("Incorrect node ID");


            IPAddress? ip = this.Request.HttpContext.Connection.RemoteIpAddress;

            if (ip == null) _logger.LogError($"Failed to get node IP from HttpContext");


            double max = 2000.0;

            double newVal = ((4096 - moistureval) / max) * 100.0;

            newVal = newVal < 0 ? 0 : newVal > 100 ? 100 : newVal;

            _greenhouse.OnNodeData(NodeID, (int)newVal, ip);

            return Ok();
            
        }
    }
}
