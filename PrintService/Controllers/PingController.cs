using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PrintService.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class PingController(ILogger<PingController> logger) : ControllerBase
    {
        private readonly ILogger<PingController> _logger = logger;

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            _logger.LogInformation("Service was pinged.");
            return Ok("Ping successfull. PrintService is running...");
        }
    }
}
