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

                PrintCapabilitiesDto dto = new PrintCapabilitiesDto();

                dto.Collation = [.. capabilities.CollationCapability.Select(c => c.ToString())];
                dto.DeviceFontSubstitution = [.. capabilities.DeviceFontSubstitutionCapability.Select(c => c.ToString())];
                dto.Duplexing = [.. capabilities.DuplexingCapability.Select(c => c.ToString())];
                dto.InputBin = [.. capabilities.InputBinCapability.Select(c => c.ToString())];
                dto.MaxCopyCount = capabilities.MaxCopyCount;
                dto.OrientedPageMediaHeight = capabilities.OrientedPageMediaHeight;
                dto.OrientedPageMediaWidth = capabilities.OrientedPageMediaWidth;
                dto.OutputColor = [.. capabilities.OutputColorCapability.Select(c => c.ToString())];
                dto.OutputQuality = [.. capabilities.OutputQualityCapability.Select(c => c.ToString())];
                dto.PageBorderless = [.. capabilities.PageBorderlessCapability.Select(c => c.ToString())];
                dto.PageImageableArea = new Dictionary<string, double?> {
                        { "ExtentHeight", capabilities.PageImageableArea.ExtentHeight },
                        { "ExtentWidth", capabilities.PageImageableArea.ExtentWidth },
                        { "OriginHeight", capabilities.PageImageableArea.OriginHeight },
                        { "OriginWidth", capabilities.PageImageableArea.OriginWidth },
                    };
                dto.PageMediaSize = [.. capabilities.PageMediaSizeCapability.Select(c => c.ToString())]; // TODO: improve serialization
                dto.PageMediaType = [.. capabilities.PageMediaTypeCapability.Select(c => c.ToString())];
                dto.PageOrder = [.. capabilities.PageOrderCapability.Select(c => c.ToString())];
                dto.PageOrientation = [.. capabilities.PageOrientationCapability.Select(c => c.ToString())];
                dto.PageResolution = [.. capabilities.PageResolutionCapability.Select(c => c.ToString())]; // TODO: improve serializatio;
                dto.PageScalingFactorRange = new Dictionary<string, int?> {
                        {"MaximumScale", capabilities.PageScalingFactorRange?.MaximumScale},
                        {"MinimumScale", capabilities.PageScalingFactorRange?.MinimumScale},
                    };
                dto.PagesPerSheet = [.. capabilities.PagesPerSheetCapability.Select(c => c.ToString())];
                dto.PagesPerSheetDirection = [.. capabilities.PagesPerSheetDirectionCapability.Select(c => c.ToString())];
                dto.PhotoPrintingIntent = [.. capabilities.PhotoPrintingIntentCapability.Select(c => c.ToString())];
                dto.Stapling = [.. capabilities.StaplingCapability.Select(c => c.ToString())];
                dto.TrueTypeFontMode = [.. capabilities.TrueTypeFontModeCapability.Select(c => c.ToString())];

                return dto;
            });
        }
    }
}
