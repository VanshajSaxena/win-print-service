using System.Printing;
using System.Windows;
using Microsoft.Extensions.Logging;
using PrintService.Exceptions;
using PrintService.Models;

namespace PrintService.Services
{
    /// <summary>
    /// Service class for print queues.
    /// </summary>
    public class PrintQueueService(ILogger<PrintQueueService> logger)
    {
        private readonly ILogger<PrintQueueService> _logger = logger;

        /// <summary>
        /// Retrieves a list of print queue names available on the local print server.
        /// </summary>
        /// <remarks>This method queries the local print server for all available print queues and returns
        /// their names. The operation is performed on the application's dispatcher thread to ensure thread
        /// safety.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of strings, where each
        /// string is the name of a print queue available on the local print server.</returns>
        public async Task<List<string>> GetPrintQueues()
        {
            _logger.LogInformation("{MethodName} was invoked", nameof(GetPrintQueues));
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    using var server = new LocalPrintServer();
                    return server.GetPrintQueues().Select(queue => queue.Name).ToList();
                }
                catch (ArgumentNullException ex)
                {
                    throw new Exception("Failed to retrive print queue.", ex);
                }
            });
        }

        /// <summary>
        /// Retrieves information about a specific print queue by its name.
        /// </summary>
        /// <remarks>This method uses the application's dispatcher to ensure thread-safe access to the
        /// print server.</remarks>
        /// <param name="queueName">The name of the print queue to retrieve. This parameter is case-insensitive.</param>
        /// <returns>A <see cref="PrintQueueDto"/> object containing details about the specified print queue,  including its full
        /// name, name, comment, description, and status.</returns>
        public async Task<PrintQueueDto> GetPrintQueue(string queueName)
        {
            _logger.LogInformation("{MethodName} was invoked with parameter: {queueName}", nameof(GetPrintQueue), queueName);
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
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
                }
                catch (InvalidOperationException ex)
                {
                    throw new PrintQueueNotFoundException($"Print queue with name '{queueName}' does not exist.", ex);
                }
                catch (ArgumentNullException ex)
                {
                    throw new Exception("Failed to retrive print queue.", ex);
                }
            });
        }

        /// <summary>
        /// Retrieves the print capabilities of a specified print queue.
        /// </summary>
        /// <remarks>This method queries the local print server to retrieve the capabilities of the
        /// specified print queue. It uses the current application's dispatcher to ensure thread safety when
        /// interacting with the print server.</remarks>
        /// <param name="queueName">The name of the print queue for which to retrieve capabilities. This value is case-insensitive.</param>
        /// <returns>A <see cref="PrintCapabilitiesDto"/> object containing the capabilities of the specified print queue, 
        /// including supported collation, duplexing, input bins, page sizes, resolutions, and other printing features.</returns>
        public async Task<PrintCapabilitiesDto> GetPrintQueueCapabilities(string queueName)
        {
            _logger.LogInformation("{MethodName} was invoked with parameter: {queueName}", nameof(GetPrintQueueCapabilities), queueName);
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    using var server = new LocalPrintServer();
                    PrintQueue current = server.GetPrintQueues().First(queue =>
                    {
                        return queue.Name.Equals(queueName, StringComparison.OrdinalIgnoreCase);
                    });
                    PrintCapabilities capabilities = current.GetPrintCapabilities();

                    PrintCapabilitiesDto dto = new()
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
                        PageImageableArea = new PageImageableAreaDto
                        {
                            ExtentHeight = capabilities.PageImageableArea.ExtentHeight,
                            ExtentWidth = capabilities.PageImageableArea.ExtentWidth,
                            OriginHeight = capabilities.PageImageableArea.OriginHeight,
                            OriginWidth = capabilities.PageImageableArea.OriginWidth,
                        },
                        PageMediaSize = [.. capabilities.PageMediaSizeCapability.Select(c => new PageMediaSizeDto
                    {
                        SizeName = c.PageMediaSizeName.ToString(),
                        Height = c.Height.ToString(),
                        Width = c.Width.ToString(),
                    })],
                        PageMediaType = [.. capabilities.PageMediaTypeCapability.Select(c => c.ToString())],
                        PageOrder = [.. capabilities.PageOrderCapability.Select(c => c.ToString())],
                        PageOrientation = [.. capabilities.PageOrientationCapability.Select(c => c.ToString())],
                        PageResolution = [.. capabilities.PageResolutionCapability.Select(c => new PageResolutionDto
                    {
                        Resolution = c.QualitativeResolution.ToString(),
                        X = c.X.ToString(),
                        Y = c.Y.ToString(),
                    })],
                        PageScalingFactorRange = new PageScalingFactorRangeDto
                        {
                            MaximumScale = capabilities.PageScalingFactorRange?.MaximumScale,
                            MinimumScale = capabilities.PageScalingFactorRange?.MinimumScale,
                        },
                        PagesPerSheet = [.. capabilities.PagesPerSheetCapability.Select(c => c.ToString())],
                        PagesPerSheetDirection = [.. capabilities.PagesPerSheetDirectionCapability.Select(c => c.ToString())],
                        PhotoPrintingIntent = [.. capabilities.PhotoPrintingIntentCapability.Select(c => c.ToString())],
                        Stapling = [.. capabilities.StaplingCapability.Select(c => c.ToString())],
                        TrueTypeFontMode = [.. capabilities.TrueTypeFontModeCapability.Select(c => c.ToString())],
                    };

                    return dto;
                }
                catch (InvalidOperationException ex)
                {
                    throw new PrintQueueNotFoundException($"Print queue with name '{queueName}' does not exist.", ex);
                }
            });
        }
    }
}
