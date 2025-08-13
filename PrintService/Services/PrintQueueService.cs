using System.Printing;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace PrintService.Services
{
    /// <summary>
    /// Service class for print queues.
    /// </summary>
    public class PrintQueueService(ILogger<PrintQueueService> logger)
    {
        private readonly ILogger<PrintQueueService> _logger = logger;

        public async Task<List<string>> GetPrintQueues()
        {
            _logger.LogInformation("{MethodName} was invoked", nameof(GetPrintQueues));
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                using var server = new LocalPrintServer();
                return server.GetPrintQueues().Select(queue => queue.Name).ToList();
            });
        }
    }
}
