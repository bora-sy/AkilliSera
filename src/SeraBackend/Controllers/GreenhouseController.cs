using Microsoft.AspNetCore.Mvc;
using SeraBackend.Greenhouse;

namespace SeraBackend.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GreenhouseController(ILogger<GreenhouseController> _logger, GreenhouseService _greenhouse) : ControllerBase
    {
        [HttpGet("solenoid")]
        public IActionResult GetSolenoid()
        {

        }


        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(_greenhouse.GetNodes());
        }

        [HttpGet("vals")]
        public IActionResult Test2()
        {
            return Ok(_greenhouse.GetNodes()[0].HumidityValues.Last().values[0]);
        }
    }
}
