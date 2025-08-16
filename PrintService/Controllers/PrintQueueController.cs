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
            var printQueues = await _printQueueService.GetPrintQueues();
            return Ok(printQueues);
        }

        [HttpGet("{queueName}")]
        public async Task<IActionResult> GetPrintQueue(string queueName)
        {
            var printQueue = await _printQueueService.GetPrintQueue(queueName);
            return Ok(printQueue);
        }

        [HttpGet("{queueName}/capabilities")]
        public async Task<IActionResult> GetPrintQueueCapabilitites(string queueName)
        {
            var printCapabilities = await _printQueueService.GetPrintQueueCapabilities(queueName);
            return Ok(printCapabilities);
        }

    }
}
