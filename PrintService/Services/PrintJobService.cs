using System.Printing;
using System.Windows;
using Microsoft.Extensions.Logging;
using PrintService.Exceptions;
using PrintService.Models;

namespace PrintService.Services
{
    public class PrintJobService(ILogger<PrintJobService> logger)
    {
        private readonly ILogger<PrintJobService> _logger = logger;
        public async Task<List<PrintJobInfoDto>> GetPrintJobsInfo(string queueName)
        {
            _logger.LogInformation("{MethodName} was invoked with parameter: {queueName}", nameof(GetPrintJobsInfo), queueName);
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    using var server = new LocalPrintServer();
                    PrintQueue current = server.GetPrintQueues().First(queue =>
                    {
                        return queue.Name.Equals(queueName, StringComparison.OrdinalIgnoreCase);
                    });

                    var printJobInfoCollection = current.GetPrintJobInfoCollection();

                    var dto = printJobInfoCollection.Select(printJobInfo => new PrintJobInfoDto()
                    {
                        JobIdentifier = printJobInfo.JobIdentifier,
                        JobStatus = printJobInfo.JobStatus.ToString(),
                        JobName = printJobInfo.Name,
                        NumberOfPages = printJobInfo.NumberOfPages,
                        NumberOfPagesPrinted = printJobInfo.NumberOfPagesPrinted,
                        PositionInPrintQueue = printJobInfo.PositionInPrintQueue,
                        Priority = printJobInfo.Priority.ToString(),
                        TimeJobSubmitted = printJobInfo.TimeJobSubmitted,
                        TimeSinceStartedPrinting = printJobInfo.TimeSinceStartedPrinting,
                    }).ToList();

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

        public async Task<PrintJobInfoDto> GetPrintJobInfo(string queueName, int jobId)
        {
            _logger.LogInformation("{MethodName} was invoked with parameter: [{queueName}, {jobId}]", nameof(GetPrintJobInfo), queueName, jobId);
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    using var server = new LocalPrintServer();
                    PrintQueue current = server.GetPrintQueues().First(queue =>
                    {
                        return queue.Name.Equals(queueName, StringComparison.OrdinalIgnoreCase);
                    });

                    var printJobInfo = current.GetJob(jobId);

                    PrintJobInfoDto dto = new()
                    {
                        JobIdentifier = printJobInfo.JobIdentifier,
                        JobStatus = printJobInfo.JobStatus.ToString(),
                        JobName = printJobInfo.Name,
                        NumberOfPages = printJobInfo.NumberOfPages,
                        NumberOfPagesPrinted = printJobInfo.NumberOfPagesPrinted,
                        PositionInPrintQueue = printJobInfo.PositionInPrintQueue,
                        Priority = printJobInfo.Priority.ToString(),
                        TimeJobSubmitted = printJobInfo.TimeJobSubmitted,
                        TimeSinceStartedPrinting = printJobInfo.TimeSinceStartedPrinting,
                    };
                    return dto;
                }
                catch (InvalidOperationException ex)
                {
                    throw new PrintQueueNotFoundException($"Print queue with name '{queueName}' does not exist.", ex);
                }
                catch (ArgumentNullException ex)
                {
                    throw new JobNotFoundException($"Job with jobId '{jobId}' does not exist.", ex);
                }
            });
        }

        public async Task<PrintJobInfoDto> AddJob(PrintJobDto printJob, string queueName)
        {
            _logger.LogInformation("{MethodName} was invoked with parameter: [{printTicket}, {jobId}]", nameof(AddJob), printJob, queueName);
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    using var server = new LocalPrintServer();
                    PrintQueue current = server.GetPrintQueues().First(queue =>
                    {
                        return queue.Name.Equals(queueName, StringComparison.OrdinalIgnoreCase);
                    });
                    PrintTicketDto? ticketDto = printJob.Ticket ?? throw new ArgumentException("PrintTicket is null.");
                    PrintTicket printTicket = ConvertToPrintTicket(ticketDto);
                    PrintSystemJobInfo? jobInfo = current.AddJob(printJob.Name, printJob.DocumentPath, printJob.FastCopy.GetValueOrDefault(true), printTicket);
                    return new PrintJobInfoDto()
                    {
                        JobIdentifier = jobInfo.JobIdentifier,
                        JobStatus = jobInfo.JobStatus.ToString(),
                        JobName = jobInfo.JobName,
                        NumberOfPages = jobInfo.NumberOfPages,
                        NumberOfPagesPrinted = jobInfo.NumberOfPagesPrinted,
                        PositionInPrintQueue = jobInfo.PositionInPrintQueue,
                        Priority = jobInfo.Priority.ToString(),
                        TimeJobSubmitted = jobInfo.TimeJobSubmitted,
                        TimeSinceStartedPrinting = jobInfo.TimeSinceStartedPrinting,
                    };
                }
                catch (InvalidOperationException ex)
                {
                    throw new PrintQueueNotFoundException($"Print queue with name '{queueName}' does not exist.", ex);
                }
                catch (ConversionFailedException)
                {
                    throw;
                }
            });
        }

        private static PrintTicket ConvertToPrintTicket(PrintTicketDto ticketDto)
        {
            PrintTicket printTicket = new();
            if (ticketDto.Collation.HasValue)
            {
                printTicket.Collation = ToPrintTicketEnum<Collation, CollationDto>(ticketDto.Collation.Value);
            }
            if (ticketDto.CopyCount.HasValue)
            {
                printTicket.CopyCount = ticketDto.CopyCount > 0 ? ticketDto.CopyCount : throw new ConversionFailedException("Cannot set CopyCount less than 1.");
            }
            if (ticketDto.DeviceFontSubstitution.HasValue)
            {
                printTicket.DeviceFontSubstitution = ToPrintTicketEnum<DeviceFontSubstitution, DeviceFontSubstitutionDto>(ticketDto.DeviceFontSubstitution.Value);
            }
            if (ticketDto.Duplexing.HasValue)
            {
                printTicket.Duplexing = ToPrintTicketEnum<Duplexing, DuplexingDto>(ticketDto.Duplexing.Value);
            }
            if (ticketDto.InputBin.HasValue)
            {
                printTicket.InputBin = ToPrintTicketEnum<InputBin, InputBinDto>(ticketDto.InputBin.Value);
            }
            if (ticketDto.OutputColor.HasValue)
            {
                printTicket.OutputColor = ToPrintTicketEnum<OutputColor, OutputColorDto>(ticketDto.OutputColor.Value);
            }
            if (ticketDto.OutputQuality.HasValue)
            {
                printTicket.OutputQuality = ToPrintTicketEnum<OutputQuality, OutputQualityDto>(ticketDto.OutputQuality.Value);
            }
            if (ticketDto.PageBorderless.HasValue)
            {
                printTicket.PageBorderless = ToPrintTicketEnum<PageBorderless, PageBorderlessDto>(ticketDto.PageBorderless.Value);
            }
            if (ticketDto.PageMediaSize != null)
            {
                printTicket.PageMediaSize = ToPrintTicketPageMediaSize(ticketDto.PageMediaSize);
            }
            if (ticketDto.PageMediaType.HasValue)
            {
                printTicket.PageMediaType = ToPrintTicketEnum<PageMediaType, PageMediaTypeDto>(ticketDto.PageMediaType.Value);
            }
            if (ticketDto.PageOrder.HasValue)
            {
                printTicket.PageOrder = ToPrintTicketEnum<PageOrder, PageOrderDto>(ticketDto.PageOrder.Value);
            }
            if (ticketDto.PageOrientation.HasValue)
            {
                printTicket.PageOrientation = ToPrintTicketEnum<PageOrientation, PageOrientationDto>(ticketDto.PageOrientation.Value);
            }
            if (ticketDto.PageResolution != null)
            {
                printTicket.PageResolution = ToPrintTicketPageResolution(ticketDto.PageResolution);
            }
            if (ticketDto.PageScalingFactor.HasValue)
            {
                printTicket.PageScalingFactor = ticketDto.PageScalingFactor > 0 ? ticketDto.PageScalingFactor : throw new ConversionFailedException("Cannot set PageScalingFactor less than 1.");
            }
            if (ticketDto.PagesPerSheet.HasValue)
            {
                printTicket.PagesPerSheet = ticketDto.PagesPerSheet > 0 ? ticketDto.PagesPerSheet : throw new ConversionFailedException("Cannot set PagesPerSheet less than 1.");
            }
            if (ticketDto.PagesPerSheetDirection.HasValue)
            {
                printTicket.PagesPerSheetDirection = ToPrintTicketEnum<PagesPerSheetDirection, PagesPerSheetDirectionDto>(ticketDto.PagesPerSheetDirection.Value);
            }
            if (ticketDto.PhotoPrintingIntent.HasValue)
            {
                printTicket.PhotoPrintingIntent = ToPrintTicketEnum<PhotoPrintingIntent, PhotoPrintingIntentDto>(ticketDto.PhotoPrintingIntent.Value);
            }
            if (ticketDto.Stapling.HasValue)
            {
                printTicket.Stapling = ToPrintTicketEnum<Stapling, StaplingDto>(ticketDto.Stapling.Value);
            }
            if (ticketDto.TrueTypeFontMode.HasValue)
            {
                printTicket.TrueTypeFontMode = ToPrintTicketEnum<TrueTypeFontMode, TrueTypeFontModeDto>(ticketDto.TrueTypeFontMode.Value);
            }
            return printTicket;
        }

        private static T ToPrintTicketEnum<T, TDto>(TDto dto) where T : struct, Enum where TDto : struct, Enum
        {
            if (Enum.TryParse(dto.ToString(), out T result))
            {
                return result;
            }
            throw new ConversionFailedException($"Invalid or unknown value: {dto}");
        }

        private static PageResolution ToPrintTicketPageResolution(PageResolutionTicketDto dto)
        {
            PageQualitativeResolutionDto? resolution = dto.Resolution;
            if (!resolution.HasValue)
            {
                if (!dto.X.HasValue)
                {
                    throw new ConversionFailedException("PageResolution.X cannot be null, when PageResolution.PageQualitativeResolution is null.");
                }
                if (!dto.Y.HasValue)
                {
                    throw new ConversionFailedException("PageResolution.Y cannot be null, when PageResolution.PageQualitativeResolution is null.");
                }
                int X = dto.X.Value;
                int Y = dto.Y.Value;
                return new PageResolution(X, Y);
            }

            PageQualitativeResolution qualitativeResolution = ToPrintTicketEnum<PageQualitativeResolution, PageQualitativeResolutionDto>(resolution.Value);

            if (dto.X.HasValue && dto.Y.HasValue)
            {
                int X = dto.X.Value;
                int Y = dto.Y.Value;
                return new PageResolution(X, Y, qualitativeResolution);
            }
            return new PageResolution(qualitativeResolution);
        }

        private static PageMediaSize ToPrintTicketPageMediaSize(PageMediaSizeTicketDto dto)
        {
            PageMediaSizeNameDto? name = dto.Name;
            if (!name.HasValue)
            {
                if (!dto.Height.HasValue)
                {
                    throw new ConversionFailedException("PageMediaSize.Height cannot be null, when PageMediaSize.PageMediaSizeName is null.");
                }
                if (!dto.Width.HasValue)
                {
                    throw new ConversionFailedException("PageMediaSize.Width cannot be null, when PageMediaSize.PageMediaSizeName is null.");
                }
                double width = dto.Width.Value;
                double height = dto.Height.Value;
                return new PageMediaSize(width, height);
            }

            PageMediaSizeName sizeName = ToPrintTicketEnum<PageMediaSizeName, PageMediaSizeNameDto>(name.Value);

            if (dto.Height.HasValue && dto.Width.HasValue)
            {
                double width = dto.Width.Value;
                double height = dto.Height.Value;
                return new PageMediaSize(sizeName, width, height);
            }
            return new PageMediaSize(sizeName);
        }
    }
}

