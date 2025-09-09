using Microsoft.AspNetCore.Mvc;
using PrintService.Models;
using PrintService.Services;

namespace PrintService.Controllers
{
    [ApiController]
    [Route("api/v1/printqueues")]
    public class PrintQueueController(PrintQueueService printQueueService, PrintJobService printJobService) : ControllerBase
    {
        private readonly PrintQueueService _printQueueService = printQueueService;
        private readonly PrintJobService _printJobService = printJobService;

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

        [HttpGet("{queueName}/jobs")]
        public async Task<IActionResult> GetJobs(string queueName)
        {
            var printJobs = await _printJobService.GetPrintJobsInfo(queueName);
            return Ok(printJobs);
        }

        [HttpGet("{queueName}/jobs/{jobId}")]
        public async Task<IActionResult> GetJobs(string queueName, int jobId)
        {
            var printJob = await _printJobService.GetPrintJobInfo(queueName, jobId);
            return Ok(printJob);
        }

        [HttpPost("{queueName}/jobs")]
        public async Task<IActionResult> AddJob(PrintJobDto printJobDto, string queueName)
        {
            var printJobInfo = await _printJobService.AddJob(printJobDto, queueName);
            return Ok(printJobInfo);
        }

    }
}
