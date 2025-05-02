using Microsoft.AspNetCore.Mvc;
using SeraBackend.Controllers.DTO;
using SeraBackend.Greenhouse;

namespace SeraBackend.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GreenhouseController(ILogger<GreenhouseController> _logger, GreenhouseService _greenhouse) : ControllerBase
    {

        [HttpGet("")]
        public IActionResult GreenhouseInfo()
        {
            Node[] nodes = _greenhouse.GetNodes();

            List<int> activeNodeIDs = new();
            List<NodeMoisture[]> moistures = new();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null || !nodes[i].Connected) continue;

                activeNodeIDs.Add(i);

                moistures.Add(nodes[i].MoistureValues);
            }

            return Ok(new GreenhouseData()
            {
                ActiveNodeIDs = activeNodeIDs.ToArray(),
                NodeMoistures = moistures.ToArray()
            });

        }
    }
}
