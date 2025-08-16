using System.Printing;
using System.Windows;
using Microsoft.Extensions.Logging;
using PrintService.Models;

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

        public async Task<PrintQueueDto> GetPrintQueue(string queueName)
        {
            _logger.LogInformation("{MethodName} was invoked", nameof(GetPrintQueues));
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                using var server = new LocalPrintServer();
                PrintQueue current = server.GetPrintQueues().First(queue =>
                {
                    return queue.Name.Equals(queueName, StringComparison.OrdinalIgnoreCase);
                });
                PrintQueueDto dto = new PrintQueueDto() with
                {
                    FullName = current.FullName,
                    Name = current.Name,
                    Comment = current.Comment,
                    Description = current.Description,
                    Status = current.QueueStatus.ToString(),
                };
                return dto;
            });
        }

        public async Task<PrintCapabilitiesDto> GetPrintQueueCapabilities(string queueName)
        {
            _logger.LogInformation("{MethodName} was invoked", nameof(GetPrintQueueCapabilities));
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                using var server = new LocalPrintServer();
                PrintQueue current = server.GetPrintQueues().First(queue =>
                {
                    return queue.Name.Equals(queueName, StringComparison.OrdinalIgnoreCase);
                });
                PrintCapabilities capabilities = current.GetPrintCapabilities();

                PrintCapabilitiesDto dto = new PrintCapabilitiesDto() with
                {
                    Collation = [.. capabilities.CollationCapability.Select(c => c.ToString())],
                    DeviceFontSubstitution = [.. capabilities.DeviceFontSubstitutionCapability.Select(c => c.ToString())],
                    Duplexing = [.. capabilities.DuplexingCapability.Select(c => c.ToString())],
                    InputBin = [.. capabilities.InputBinCapability.Select(c => c.ToString())],
                    MaxCopyCount = capabilities.MaxCopyCount,
                    OrientedPageMediaHeight = capabilities.OrientedPageMediaHeight,
                    OrientedPageMediaWidth = capabilities.OrientedPageMediaWidth,
                    OutputColor = [.. capabilities.OutputColorCapability.Select(c => c.ToString())],
                    OutputQuality = [.. capabilities.OutputQualityCapability.Select(c => c.ToString())],
                    PageBorderless = [.. capabilities.PageBorderlessCapability.Select(c => c.ToString())],
                    PageImageableArea = new Dictionary<string, double?> {
                        { "ExtentHeight", capabilities.PageImageableArea.ExtentHeight },
                        { "ExtentWidth", capabilities.PageImageableArea.ExtentWidth },
                        { "OriginHeight", capabilities.PageImageableArea.OriginHeight },
                        { "OriginWidth", capabilities.PageImageableArea.OriginWidth },
                    },
                    PageMediaSize = [.. capabilities.PageMediaSizeCapability.Select(c => c.ToString())], // TODO: improve serialization
                    PageMediaType = [.. capabilities.PageMediaTypeCapability.Select(c => c.ToString())],
                    PageOrder = [.. capabilities.PageOrderCapability.Select(c => c.ToString())],
                    PageOrientation = [.. capabilities.PageOrientationCapability.Select(c => c.ToString())],
                    PageResolution = [.. capabilities.PageResolutionCapability.Select(c => c.ToString())], // TODO: improve serializatio;
                    PageScalingFactorRange = new Dictionary<string, int?> {
                        {"MaximumScale", capabilities.PageScalingFactorRange?.MaximumScale},
                        {"MinimumScale", capabilities.PageScalingFactorRange?.MinimumScale},
                    },
                    PagesPerSheet = [.. capabilities.PagesPerSheetCapability.Select(c => c.ToString())],
                    PagesPerSheetDirection = [.. capabilities.PagesPerSheetDirectionCapability.Select(c => c.ToString())],
                    PhotoPrintingIntent = [.. capabilities.PhotoPrintingIntentCapability.Select(c => c.ToString())],
                    Stapling = [.. capabilities.StaplingCapability.Select(c => c.ToString())],
                    TrueTypeFontMode = [.. capabilities.TrueTypeFontModeCapability.Select(c => c.ToString())],
                };

                return dto;
            });
        }
    }
}
