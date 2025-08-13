using Microsoft.AspNetCore.Mvc;
using PrintService.Services;

namespace PrintService.Controllers
{
    [ApiController]
    [Route("api/v1/printqueues")]
    public class PrintQueueController(PrintQueueService printQueueService) : ControllerBase
    {
        private readonly PrintQueueService _printQueueService = printQueueService;

        [HttpGet]
        public async Task<IActionResult> GetPrintQueues()
        {
            var printqueues = await _printQueueService.GetPrintQueues();
            return Ok(printqueues);
        }
    }
}
