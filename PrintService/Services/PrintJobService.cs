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
                catch (Exception ex)
                {
                    throw new Exception("An unhandled exception occured.", ex);
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
                catch (Exception ex)
                {
                    throw new Exception("An unhandled exception occured.", ex);
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
                    PrintTicketDto? ticketDto = printJob.Ticket;
                    if (ticketDto == null)
                    {
                        throw new ArgumentException("PrintTicket is null.");
                    }
                    PrintTicket printTicket = ConvertToPrintTicket(ticketDto);
                    PrintSystemJobInfo? jobInfo = current.AddJob(printJob.Name, printJob.DocumentPath, false, printTicket);
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
                catch (Exception ex)
                {
                    throw new Exception("An unhandled exception occured.", ex);
                }
            });
        }

        private PrintTicket ConvertToPrintTicket(PrintTicketDto ticketDto)
        {
            PrintTicket printTicket = new();
            try
            {
                if (ticketDto.Collation.HasValue)
                {
                    printTicket.Collation = ToPrintTicketCollation(ticketDto.Collation.Value);
                }
                if (ticketDto.CopyCount.HasValue)
                {
                    printTicket.CopyCount = ticketDto.CopyCount > 0 ? ticketDto.CopyCount : throw new ConversionFailedException("Cannot set CopyCount less than 1.");
                }
                if (ticketDto.DeviceFontSubstitution.HasValue)
                {
                    printTicket.DeviceFontSubstitution = ToPrintTicketDeviceFontSubstitution(ticketDto.DeviceFontSubstitution.Value);
                }
                if (ticketDto.Duplexing.HasValue)
                {
                    printTicket.Duplexing = ToPrintTicketDuplixing(ticketDto.Duplexing.Value);
                }
                if (ticketDto.InputBin.HasValue) { printTicket.InputBin = ToPrintTicketInputBin(ticketDto.InputBin.Value); }
                if (ticketDto.OutputColor.HasValue) { printTicket.OutputColor = ToPrintTicketOutputColor(ticketDto.OutputColor.Value); }
                if (ticketDto.OutputQuality.HasValue) { printTicket.OutputQuality = ToPrintTicketOutputQuality(ticketDto.OutputQuality.Value); }
                if (ticketDto.PageBorderless.HasValue) { printTicket.PageBorderless = ToPrintTicketPageBorderless(ticketDto.PageBorderless.Value); }
                if (ticketDto.PageMediaSize != null) { printTicket.PageMediaSize = ToPrintTicketPageMediaSize(ticketDto.PageMediaSize); }
                if (ticketDto.PageMediaType.HasValue) { printTicket.PageMediaType = ToPrintTicketPageMediaType(ticketDto.PageMediaType.Value); }
                if (ticketDto.PageOrder.HasValue) { printTicket.PageOrder = ToPrintTicketPageOrder(ticketDto.PageOrder.Value); }
                if (!ticketDto.PageOrientation.HasValue)
                {
                }
                else
                { printTicket.PageOrientation = ToPrintTicketPageOrientation(ticketDto.PageOrientation.Value); }

                if (ticketDto.PageResolution != null) { printTicket.PageResolution = ToPrintTicketPageResolution(ticketDto.PageResolution); }
                if (ticketDto.PageScalingFactor.HasValue)
                {
                    printTicket.PageScalingFactor = ticketDto.PageScalingFactor > 0 ? ticketDto.PageScalingFactor : throw new ConversionFailedException("Cannot set PageScalingFactor less than 1.");
                }
                if (ticketDto.PagesPerSheet.HasValue)
                {
                    printTicket.PagesPerSheet = ticketDto.PagesPerSheet > 0 ? ticketDto.PagesPerSheet : throw new ConversionFailedException("Cannot set PagesPerSheet less than 1.");
                }
                if (ticketDto.PagesPerSheetDirection.HasValue) { printTicket.PagesPerSheetDirection = ToPrintTicketPagesPerSheetDirection(ticketDto.PagesPerSheetDirection.Value); }
                if (ticketDto.PhotoPrintingIntent.HasValue) { printTicket.PhotoPrintingIntent = ToPrintTicketPhotoPrintingIntent(ticketDto.PhotoPrintingIntent.Value); }
                if (ticketDto.Stapling.HasValue) { printTicket.Stapling = ToPrintTicketStapling(ticketDto.Stapling.Value); }
                if (ticketDto.TrueTypeFontMode.HasValue) { printTicket.TrueTypeFontMode = ToPrintTicketTrueTypeFontMode(ticketDto.TrueTypeFontMode.Value); }
                return printTicket;
            }
            catch (Exception)
            {
                throw;
            }
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

            PageQualitativeResolution qualitativeResolution = resolution switch
            {
                PageQualitativeResolutionDto.Default => PageQualitativeResolution.Default,
                PageQualitativeResolutionDto.Draft => PageQualitativeResolution.Draft,
                PageQualitativeResolutionDto.High => PageQualitativeResolution.High,
                PageQualitativeResolutionDto.Normal => PageQualitativeResolution.Normal,
                PageQualitativeResolutionDto.Other => PageQualitativeResolution.Other,
                PageQualitativeResolutionDto.Unknown => PageQualitativeResolution.Unknown,
                _ => throw new ConversionFailedException($"Invalid or unknown PageQualitativeResolution value: {resolution}")
            };

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

            PageMediaSizeName sizeName = name switch
            {
                PageMediaSizeNameDto.ISOA0 => PageMediaSizeName.ISOA0,
                PageMediaSizeNameDto.ISOA1 => PageMediaSizeName.ISOA1,
                PageMediaSizeNameDto.ISOA10 => PageMediaSizeName.ISOA10,
                PageMediaSizeNameDto.ISOA2 => PageMediaSizeName.ISOA2,
                PageMediaSizeNameDto.ISOA3 => PageMediaSizeName.ISOA3,
                PageMediaSizeNameDto.ISOA3Rotated => PageMediaSizeName.ISOA3Rotated,
                PageMediaSizeNameDto.ISOA3Extra => PageMediaSizeName.ISOA3Extra,
                PageMediaSizeNameDto.ISOA4 => PageMediaSizeName.ISOA4,
                PageMediaSizeNameDto.ISOA4Rotated => PageMediaSizeName.ISOA4Rotated,
                PageMediaSizeNameDto.ISOA4Extra => PageMediaSizeName.ISOA4Extra,
                PageMediaSizeNameDto.ISOA5 => PageMediaSizeName.ISOA5,
                PageMediaSizeNameDto.ISOA5Rotated => PageMediaSizeName.ISOA5Rotated,
                PageMediaSizeNameDto.ISOA5Extra => PageMediaSizeName.ISOA5Extra,
                PageMediaSizeNameDto.ISOA6 => PageMediaSizeName.ISOA6,
                PageMediaSizeNameDto.ISOA6Rotated => PageMediaSizeName.ISOA6Rotated,
                PageMediaSizeNameDto.ISOA7 => PageMediaSizeName.ISOA7,
                PageMediaSizeNameDto.ISOA8 => PageMediaSizeName.ISOA8,
                PageMediaSizeNameDto.ISOA9 => PageMediaSizeName.ISOA9,
                PageMediaSizeNameDto.ISOB0 => PageMediaSizeName.ISOB0,
                PageMediaSizeNameDto.ISOB1 => PageMediaSizeName.ISOB1,
                PageMediaSizeNameDto.ISOB10 => PageMediaSizeName.ISOB10,
                PageMediaSizeNameDto.ISOB2 => PageMediaSizeName.ISOB2,
                PageMediaSizeNameDto.ISOB3 => PageMediaSizeName.ISOB3,
                PageMediaSizeNameDto.ISOB4 => PageMediaSizeName.ISOB4,
                PageMediaSizeNameDto.ISOB4Envelope => PageMediaSizeName.ISOB4Envelope,
                PageMediaSizeNameDto.ISOB5Envelope => PageMediaSizeName.ISOB5Envelope,
                PageMediaSizeNameDto.ISOB5Extra => PageMediaSizeName.ISOB5Extra,
                PageMediaSizeNameDto.ISOB7 => PageMediaSizeName.ISOB7,
                PageMediaSizeNameDto.ISOB8 => PageMediaSizeName.ISOB8,
                PageMediaSizeNameDto.ISOB9 => PageMediaSizeName.ISOB9,
                PageMediaSizeNameDto.ISOC0 => PageMediaSizeName.ISOC0,
                PageMediaSizeNameDto.ISOC1 => PageMediaSizeName.ISOC1,
                PageMediaSizeNameDto.ISOC10 => PageMediaSizeName.ISOC10,
                PageMediaSizeNameDto.ISOC2 => PageMediaSizeName.ISOC2,
                PageMediaSizeNameDto.ISOC3 => PageMediaSizeName.ISOC3,
                PageMediaSizeNameDto.ISOC3Envelope => PageMediaSizeName.ISOC3Envelope,
                PageMediaSizeNameDto.ISOC4 => PageMediaSizeName.ISOC4,
                PageMediaSizeNameDto.ISOC4Envelope => PageMediaSizeName.ISOC4Envelope,
                PageMediaSizeNameDto.ISOC5 => PageMediaSizeName.ISOC5,
                PageMediaSizeNameDto.ISOC5Envelope => PageMediaSizeName.ISOC5Envelope,
                PageMediaSizeNameDto.ISOC6 => PageMediaSizeName.ISOC6,
                PageMediaSizeNameDto.ISOC6Envelope => PageMediaSizeName.ISOC6Envelope,
                PageMediaSizeNameDto.ISOC6C5Envelope => PageMediaSizeName.ISOC6C5Envelope,
                PageMediaSizeNameDto.ISOC7 => PageMediaSizeName.ISOC7,
                PageMediaSizeNameDto.ISOC8 => PageMediaSizeName.ISOC8,
                PageMediaSizeNameDto.ISOC9 => PageMediaSizeName.ISOC9,
                PageMediaSizeNameDto.ISODLEnvelope => PageMediaSizeName.ISODLEnvelope,
                PageMediaSizeNameDto.ISODLEnvelopeRotated => PageMediaSizeName.ISODLEnvelopeRotated,
                PageMediaSizeNameDto.ISOSRA3 => PageMediaSizeName.ISOSRA3,
                PageMediaSizeNameDto.JapanQuadrupleHagakiPostcard => PageMediaSizeName.JapanQuadrupleHagakiPostcard,
                PageMediaSizeNameDto.JISB0 => PageMediaSizeName.JISB0,
                PageMediaSizeNameDto.JISB1 => PageMediaSizeName.JISB1,
                PageMediaSizeNameDto.JISB10 => PageMediaSizeName.JISB10,
                PageMediaSizeNameDto.JISB2 => PageMediaSizeName.JISB2,
                PageMediaSizeNameDto.JISB3 => PageMediaSizeName.JISB3,
                PageMediaSizeNameDto.JISB4 => PageMediaSizeName.JISB4,
                PageMediaSizeNameDto.JISB4Rotated => PageMediaSizeName.JISB4Rotated,
                PageMediaSizeNameDto.JISB5 => PageMediaSizeName.JISB5,
                PageMediaSizeNameDto.JISB5Rotated => PageMediaSizeName.JISB5Rotated,
                PageMediaSizeNameDto.JISB6 => PageMediaSizeName.JISB6,
                PageMediaSizeNameDto.JISB6Rotated => PageMediaSizeName.JISB6Rotated,
                PageMediaSizeNameDto.JISB7 => PageMediaSizeName.JISB7,
                PageMediaSizeNameDto.JISB8 => PageMediaSizeName.JISB8,
                PageMediaSizeNameDto.JISB9 => PageMediaSizeName.JISB9,
                PageMediaSizeNameDto.JapanChou3Envelope => PageMediaSizeName.JapanChou3Envelope,
                PageMediaSizeNameDto.JapanChou3EnvelopeRotated => PageMediaSizeName.JapanChou3EnvelopeRotated,
                PageMediaSizeNameDto.JapanChou4Envelope => PageMediaSizeName.JapanChou4Envelope,
                PageMediaSizeNameDto.JapanChou4EnvelopeRotated => PageMediaSizeName.JapanChou4EnvelopeRotated,
                PageMediaSizeNameDto.JapanHagakiPostcard => PageMediaSizeName.JapanHagakiPostcard,
                PageMediaSizeNameDto.JapanHagakiPostcardRotated => PageMediaSizeName.JapanHagakiPostcardRotated,
                PageMediaSizeNameDto.JapanKaku2Envelope => PageMediaSizeName.JapanKaku2Envelope,
                PageMediaSizeNameDto.JapanKaku2EnvelopeRotated => PageMediaSizeName.JapanKaku2EnvelopeRotated,
                PageMediaSizeNameDto.JapanKaku3Envelope => PageMediaSizeName.JapanKaku3Envelope,
                PageMediaSizeNameDto.JapanKaku3EnvelopeRotated => PageMediaSizeName.JapanKaku3EnvelopeRotated,
                PageMediaSizeNameDto.JapanYou4Envelope => PageMediaSizeName.JapanYou4Envelope,
                PageMediaSizeNameDto.NorthAmerica10x11 => PageMediaSizeName.NorthAmerica10x11,
                PageMediaSizeNameDto.NorthAmerica10x14 => PageMediaSizeName.NorthAmerica10x14,
                PageMediaSizeNameDto.NorthAmerica11x17 => PageMediaSizeName.NorthAmerica11x17,
                PageMediaSizeNameDto.NorthAmerica9x11 => PageMediaSizeName.NorthAmerica9x11,
                PageMediaSizeNameDto.NorthAmericaArchitectureASheet => PageMediaSizeName.NorthAmericaArchitectureASheet,
                PageMediaSizeNameDto.NorthAmericaArchitectureBSheet => PageMediaSizeName.NorthAmericaArchitectureBSheet,
                PageMediaSizeNameDto.NorthAmericaArchitectureCSheet => PageMediaSizeName.NorthAmericaArchitectureCSheet,
                PageMediaSizeNameDto.NorthAmericaArchitectureDSheet => PageMediaSizeName.NorthAmericaArchitectureDSheet,
                PageMediaSizeNameDto.NorthAmericaArchitectureESheet => PageMediaSizeName.NorthAmericaArchitectureESheet,
                PageMediaSizeNameDto.NorthAmericaCSheet => PageMediaSizeName.NorthAmericaCSheet,
                PageMediaSizeNameDto.NorthAmericaDSheet => PageMediaSizeName.NorthAmericaDSheet,
                PageMediaSizeNameDto.NorthAmericaESheet => PageMediaSizeName.NorthAmericaESheet,
                PageMediaSizeNameDto.NorthAmericaExecutive => PageMediaSizeName.NorthAmericaExecutive,
                PageMediaSizeNameDto.NorthAmericaGermanLegalFanfold => PageMediaSizeName.NorthAmericaGermanLegalFanfold,
                PageMediaSizeNameDto.NorthAmericaGermanStandardFanfold => PageMediaSizeName.NorthAmericaGermanStandardFanfold,
                PageMediaSizeNameDto.NorthAmericaLegal => PageMediaSizeName.NorthAmericaLegal,
                PageMediaSizeNameDto.NorthAmericaLegalExtra => PageMediaSizeName.NorthAmericaLegalExtra,
                PageMediaSizeNameDto.NorthAmericaLetter => PageMediaSizeName.NorthAmericaLetter,
                PageMediaSizeNameDto.NorthAmericaLetterRotated => PageMediaSizeName.NorthAmericaLetterRotated,
                PageMediaSizeNameDto.NorthAmericaLetterExtra => PageMediaSizeName.NorthAmericaLetterExtra,
                PageMediaSizeNameDto.NorthAmericaLetterPlus => PageMediaSizeName.NorthAmericaLetterPlus,
                PageMediaSizeNameDto.NorthAmericaMonarchEnvelope => PageMediaSizeName.NorthAmericaMonarchEnvelope,
                PageMediaSizeNameDto.NorthAmericaNote => PageMediaSizeName.NorthAmericaNote,
                PageMediaSizeNameDto.NorthAmericaNumber10Envelope => PageMediaSizeName.NorthAmericaNumber10Envelope,
                PageMediaSizeNameDto.NorthAmericaNumber10EnvelopeRotated => PageMediaSizeName.NorthAmericaNumber10EnvelopeRotated,
                PageMediaSizeNameDto.NorthAmericaNumber9Envelope => PageMediaSizeName.NorthAmericaNumber9Envelope,
                PageMediaSizeNameDto.NorthAmericaNumber11Envelope => PageMediaSizeName.NorthAmericaNumber11Envelope,
                PageMediaSizeNameDto.NorthAmericaNumber12Envelope => PageMediaSizeName.NorthAmericaNumber12Envelope,
                PageMediaSizeNameDto.NorthAmericaNumber14Envelope => PageMediaSizeName.NorthAmericaNumber14Envelope,
                PageMediaSizeNameDto.NorthAmericaPersonalEnvelope => PageMediaSizeName.NorthAmericaPersonalEnvelope,
                PageMediaSizeNameDto.NorthAmericaQuarto => PageMediaSizeName.NorthAmericaQuarto,
                PageMediaSizeNameDto.NorthAmericaStatement => PageMediaSizeName.NorthAmericaStatement,
                PageMediaSizeNameDto.NorthAmericaSuperA => PageMediaSizeName.NorthAmericaSuperA,
                PageMediaSizeNameDto.NorthAmericaSuperB => PageMediaSizeName.NorthAmericaSuperB,
                PageMediaSizeNameDto.NorthAmericaTabloid => PageMediaSizeName.NorthAmericaTabloid,
                PageMediaSizeNameDto.NorthAmericaTabloidExtra => PageMediaSizeName.NorthAmericaTabloidExtra,
                PageMediaSizeNameDto.OtherMetricA4Plus => PageMediaSizeName.OtherMetricA4Plus,
                PageMediaSizeNameDto.OtherMetricA3Plus => PageMediaSizeName.OtherMetricA3Plus,
                PageMediaSizeNameDto.OtherMetricFolio => PageMediaSizeName.OtherMetricFolio,
                PageMediaSizeNameDto.OtherMetricInviteEnvelope => PageMediaSizeName.OtherMetricInviteEnvelope,
                PageMediaSizeNameDto.OtherMetricItalianEnvelope => PageMediaSizeName.OtherMetricItalianEnvelope,
                PageMediaSizeNameDto.PRC1Envelope => PageMediaSizeName.PRC1Envelope,
                PageMediaSizeNameDto.PRC1EnvelopeRotated => PageMediaSizeName.PRC1EnvelopeRotated,
                PageMediaSizeNameDto.PRC10Envelope => PageMediaSizeName.PRC10Envelope,
                PageMediaSizeNameDto.PRC10EnvelopeRotated => PageMediaSizeName.PRC10EnvelopeRotated,
                PageMediaSizeNameDto.PRC16K => PageMediaSizeName.PRC16K,
                PageMediaSizeNameDto.PRC16KRotated => PageMediaSizeName.PRC16KRotated,
                PageMediaSizeNameDto.PRC2Envelope => PageMediaSizeName.PRC2Envelope,
                PageMediaSizeNameDto.PRC2EnvelopeRotated => PageMediaSizeName.PRC2EnvelopeRotated,
                PageMediaSizeNameDto.PRC32K => PageMediaSizeName.PRC32K,
                PageMediaSizeNameDto.PRC32KRotated => PageMediaSizeName.PRC32KRotated,
                PageMediaSizeNameDto.PRC32KBig => PageMediaSizeName.PRC32KBig,
                PageMediaSizeNameDto.PRC3Envelope => PageMediaSizeName.PRC3Envelope,
                PageMediaSizeNameDto.PRC3EnvelopeRotated => PageMediaSizeName.PRC3EnvelopeRotated,
                PageMediaSizeNameDto.PRC4Envelope => PageMediaSizeName.PRC4Envelope,
                PageMediaSizeNameDto.PRC4EnvelopeRotated => PageMediaSizeName.PRC4EnvelopeRotated,
                PageMediaSizeNameDto.PRC5Envelope => PageMediaSizeName.PRC5Envelope,
                PageMediaSizeNameDto.PRC5EnvelopeRotated => PageMediaSizeName.PRC5EnvelopeRotated,
                PageMediaSizeNameDto.PRC6Envelope => PageMediaSizeName.PRC6Envelope,
                PageMediaSizeNameDto.PRC6EnvelopeRotated => PageMediaSizeName.PRC6EnvelopeRotated,
                PageMediaSizeNameDto.PRC7Envelope => PageMediaSizeName.PRC7Envelope,
                PageMediaSizeNameDto.PRC7EnvelopeRotated => PageMediaSizeName.PRC7EnvelopeRotated,
                PageMediaSizeNameDto.PRC8Envelope => PageMediaSizeName.PRC8Envelope,
                PageMediaSizeNameDto.PRC8EnvelopeRotated => PageMediaSizeName.PRC8EnvelopeRotated,
                PageMediaSizeNameDto.PRC9Envelope => PageMediaSizeName.PRC9Envelope,
                PageMediaSizeNameDto.PRC9EnvelopeRotated => PageMediaSizeName.PRC9EnvelopeRotated,
                PageMediaSizeNameDto.Roll04Inch => PageMediaSizeName.Roll04Inch,
                PageMediaSizeNameDto.Roll06Inch => PageMediaSizeName.Roll06Inch,
                PageMediaSizeNameDto.Roll08Inch => PageMediaSizeName.Roll08Inch,
                PageMediaSizeNameDto.Roll12Inch => PageMediaSizeName.Roll12Inch,
                PageMediaSizeNameDto.Roll15Inch => PageMediaSizeName.Roll15Inch,
                PageMediaSizeNameDto.Roll18Inch => PageMediaSizeName.Roll18Inch,
                PageMediaSizeNameDto.Roll22Inch => PageMediaSizeName.Roll22Inch,
                PageMediaSizeNameDto.Roll24Inch => PageMediaSizeName.Roll24Inch,
                PageMediaSizeNameDto.Roll30Inch => PageMediaSizeName.Roll30Inch,
                PageMediaSizeNameDto.Roll36Inch => PageMediaSizeName.Roll36Inch,
                PageMediaSizeNameDto.Roll54Inch => PageMediaSizeName.Roll54Inch,
                PageMediaSizeNameDto.JapanDoubleHagakiPostcard => PageMediaSizeName.JapanDoubleHagakiPostcard,
                PageMediaSizeNameDto.JapanDoubleHagakiPostcardRotated => PageMediaSizeName.JapanDoubleHagakiPostcardRotated,
                PageMediaSizeNameDto.JapanLPhoto => PageMediaSizeName.JapanLPhoto,
                PageMediaSizeNameDto.Japan2LPhoto => PageMediaSizeName.Japan2LPhoto,
                PageMediaSizeNameDto.JapanYou1Envelope => PageMediaSizeName.JapanYou1Envelope,
                PageMediaSizeNameDto.JapanYou2Envelope => PageMediaSizeName.JapanYou2Envelope,
                PageMediaSizeNameDto.JapanYou3Envelope => PageMediaSizeName.JapanYou3Envelope,
                PageMediaSizeNameDto.JapanYou4EnvelopeRotated => PageMediaSizeName.JapanYou4EnvelopeRotated,
                PageMediaSizeNameDto.JapanYou6Envelope => PageMediaSizeName.JapanYou6Envelope,
                PageMediaSizeNameDto.JapanYou6EnvelopeRotated => PageMediaSizeName.JapanYou6EnvelopeRotated,
                PageMediaSizeNameDto.NorthAmerica4x6 => PageMediaSizeName.NorthAmerica4x6,
                PageMediaSizeNameDto.NorthAmerica4x8 => PageMediaSizeName.NorthAmerica4x8,
                PageMediaSizeNameDto.NorthAmerica5x7 => PageMediaSizeName.NorthAmerica5x7,
                PageMediaSizeNameDto.NorthAmerica8x10 => PageMediaSizeName.NorthAmerica8x10,
                PageMediaSizeNameDto.NorthAmerica10x12 => PageMediaSizeName.NorthAmerica10x12,
                PageMediaSizeNameDto.NorthAmerica14x17 => PageMediaSizeName.NorthAmerica14x17,
                PageMediaSizeNameDto.BusinessCard => PageMediaSizeName.BusinessCard,
                PageMediaSizeNameDto.CreditCard => PageMediaSizeName.CreditCard,
                PageMediaSizeNameDto.Unknown => PageMediaSizeName.Unknown,
                _ => throw new ConversionFailedException($"Invalid or unknown PageMediaSizeName value: {name}"),
            };
            if (dto.Height.HasValue && dto.Width.HasValue)
            {
                double width = dto.Width.Value;
                double height = dto.Height.Value;
                return new PageMediaSize(sizeName, width, height);
            }
            return new PageMediaSize(sizeName);
        }

        private static TrueTypeFontMode? ToPrintTicketTrueTypeFontMode(TrueTypeFontModeDto trueTypeFontMode)
        {
            switch (trueTypeFontMode)
            {
                case TrueTypeFontModeDto.Automatic:
                    return TrueTypeFontMode.Automatic;
                case TrueTypeFontModeDto.DownloadAsNativeTrueTypeFont:
                    return TrueTypeFontMode.DownloadAsNativeTrueTypeFont;
                case TrueTypeFontModeDto.DownloadAsOutlineFont:
                    return TrueTypeFontMode.DownloadAsOutlineFont;
                case TrueTypeFontModeDto.DownloadAsRasterFont:
                    return TrueTypeFontMode.DownloadAsRasterFont;
                case TrueTypeFontModeDto.RenderAsBitmap:
                    return TrueTypeFontMode.RenderAsBitmap;
                case TrueTypeFontModeDto.Unknown:
                    return TrueTypeFontMode.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown TrueTypeFontModeDto value: {trueTypeFontMode}");

            }
        }

        private static Stapling? ToPrintTicketStapling(StaplingDto stabling)
        {
            switch (stabling)
            {
                case StaplingDto.None:
                    return Stapling.None;
                case StaplingDto.StapleTopRight:
                    return Stapling.StapleTopRight;
                case StaplingDto.StapleTopLeft:
                    return Stapling.StapleTopLeft;
                case StaplingDto.StapleDualTop:
                    return Stapling.StapleDualTop;
                case StaplingDto.StapleDualRight:
                    return Stapling.StapleDualRight;
                case StaplingDto.StapleDualLeft:
                    return Stapling.StapleDualLeft;
                case StaplingDto.StapleDualBottom:
                    return Stapling.StapleDualBottom;
                case StaplingDto.StapleBottomRight:
                    return Stapling.StapleBottomRight;
                case StaplingDto.StapleBottomLeft:
                    return Stapling.StapleBottomLeft;
                case StaplingDto.SaddleStitch:
                    return Stapling.SaddleStitch;
                case StaplingDto.Unknown:
                    return Stapling.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown Stapling value: {stabling}");
            }
        }

        private static PhotoPrintingIntent? ToPrintTicketPhotoPrintingIntent(PhotoPrintingIntentDto photoPrintingIntent)
        {
            switch (photoPrintingIntent)
            {
                case PhotoPrintingIntentDto.None:
                    return PhotoPrintingIntent.None;
                case PhotoPrintingIntentDto.PhotoBest:
                    return PhotoPrintingIntent.PhotoBest;
                case PhotoPrintingIntentDto.PhotoDraft:
                    return PhotoPrintingIntent.PhotoDraft;
                case PhotoPrintingIntentDto.PhotoStandard:
                    return PhotoPrintingIntent.PhotoStandard;
                case PhotoPrintingIntentDto.Unknown:
                    return PhotoPrintingIntent.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown PhotoPrintingIntent value: {photoPrintingIntent}");
            }
        }

        private static PagesPerSheetDirection? ToPrintTicketPagesPerSheetDirection(PagesPerSheetDirectionDto pagesPerSheetDirection)
        {
            switch (pagesPerSheetDirection)
            {
                case PagesPerSheetDirectionDto.TopRight:
                    return PagesPerSheetDirection.TopRight;
                case PagesPerSheetDirectionDto.TopLeft:
                    return PagesPerSheetDirection.TopLeft;
                case PagesPerSheetDirectionDto.BottomLeft:
                    return PagesPerSheetDirection.BottomLeft;
                case PagesPerSheetDirectionDto.BottomRight:
                    return PagesPerSheetDirection.BottomRight;
                case PagesPerSheetDirectionDto.LeftBottom:
                    return PagesPerSheetDirection.LeftBottom;
                case PagesPerSheetDirectionDto.LeftTop:
                    return PagesPerSheetDirection.LeftTop;
                case PagesPerSheetDirectionDto.RightBottom:
                    return PagesPerSheetDirection.RightBottom;
                case PagesPerSheetDirectionDto.RightTop:
                    return PagesPerSheetDirection.RightTop;
                case PagesPerSheetDirectionDto.Unknown:
                    return PagesPerSheetDirection.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown PagesPerSheetDirection value: {pagesPerSheetDirection}");

            }
        }

        private static PageOrientation? ToPrintTicketPageOrientation(PageOrientationDto pageOrientation)
        {
            switch (pageOrientation)
            {
                case PageOrientationDto.Landscape:
                    return PageOrientation.Landscape;
                case PageOrientationDto.Portrait:
                    return PageOrientation.Portrait;
                case PageOrientationDto.ReverseLandscape:
                    return PageOrientation.ReverseLandscape;
                case PageOrientationDto.ReversePortrait:
                    return PageOrientation.ReversePortrait;
                case PageOrientationDto.Unknown:
                    return PageOrientation.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown PageOrientation value: {pageOrientation}");
            }
        }

        private static PageOrder? ToPrintTicketPageOrder(PageOrderDto pageOrder)
        {
            switch (pageOrder)
            {
                case PageOrderDto.Standard:
                    return PageOrder.Standard;
                case PageOrderDto.Reverse:
                    return PageOrder.Reverse;
                case PageOrderDto.Unknown:
                    return PageOrder.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown PageOrder value: {pageOrder}");
            }
        }

        private static PageMediaType? ToPrintTicketPageMediaType(PageMediaTypeDto pageMediaType)
        {
            switch (pageMediaType)
            {
                case PageMediaTypeDto.None:
                    return PageMediaType.None;
                case PageMediaTypeDto.AutoSelect:
                    return PageMediaType.AutoSelect;
                case PageMediaTypeDto.Archival:
                    return PageMediaType.Archival;
                case PageMediaTypeDto.BackPrintFilm:
                    return PageMediaType.BackPrintFilm;
                case PageMediaTypeDto.Bond:
                    return PageMediaType.Bond;
                case PageMediaTypeDto.CardStock:
                    return PageMediaType.CardStock;
                case PageMediaTypeDto.Continuous:
                    return PageMediaType.Continuous;
                case PageMediaTypeDto.EnvelopePlain:
                    return PageMediaType.EnvelopePlain;
                case PageMediaTypeDto.EnvelopeWindow:
                    return PageMediaType.EnvelopeWindow;
                case PageMediaTypeDto.Fabric:
                    return PageMediaType.Fabric;
                case PageMediaTypeDto.HighResolution:
                    return PageMediaType.HighResolution;
                case PageMediaTypeDto.Label:
                    return PageMediaType.Label;
                case PageMediaTypeDto.MultiLayerForm:
                    return PageMediaType.MultiLayerForm;
                case PageMediaTypeDto.MultiPartForm:
                    return PageMediaType.MultiPartForm;
                case PageMediaTypeDto.Photographic:
                    return PageMediaType.Photographic;
                case PageMediaTypeDto.PhotographicFilm:
                    return PageMediaType.PhotographicFilm;
                case PageMediaTypeDto.PhotographicGlossy:
                    return PageMediaType.PhotographicGlossy;
                case PageMediaTypeDto.PhotographicHighGloss:
                    return PageMediaType.PhotographicHighGloss;
                case PageMediaTypeDto.PhotographicMatte:
                    return PageMediaType.PhotographicMatte;
                case PageMediaTypeDto.PhotographicSatin:
                    return PageMediaType.PhotographicSatin;
                case PageMediaTypeDto.PhotographicSemiGloss:
                    return PageMediaType.PhotographicSemiGloss;
                case PageMediaTypeDto.Plain:
                    return PageMediaType.Plain;
                case PageMediaTypeDto.Screen:
                    return PageMediaType.Screen;
                case PageMediaTypeDto.ScreenPaged:
                    return PageMediaType.ScreenPaged;
                case PageMediaTypeDto.Stationery:
                    return PageMediaType.Stationery;
                case PageMediaTypeDto.TabStockFull:
                    return PageMediaType.TabStockFull;
                case PageMediaTypeDto.TabStockPreCut:
                    return PageMediaType.TabStockPreCut;
                case PageMediaTypeDto.Transparency:
                    return PageMediaType.Transparency;
                case PageMediaTypeDto.TShirtTransfer:
                    return PageMediaType.TShirtTransfer;
                case PageMediaTypeDto.Unknown:
                    return PageMediaType.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown PageMediaType value: {pageMediaType}");
            }
        }

        private static PageBorderless? ToPrintTicketPageBorderless(PageBorderlessDto pageBorderless)
        {
            switch (pageBorderless)
            {
                case PageBorderlessDto.None:
                    return PageBorderless.None;
                case PageBorderlessDto.Borderless:
                    return PageBorderless.Borderless;
                case PageBorderlessDto.Unknown:
                    return PageBorderless.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown PageBorderless value: {pageBorderless}");
            }
        }

        private static OutputQuality? ToPrintTicketOutputQuality(OutputQualityDto outputQuality)
        {
            switch (outputQuality)
            {
                case OutputQualityDto.Automatic:
                    return OutputQuality.Automatic;
                case OutputQualityDto.Draft:
                    return OutputQuality.Draft;
                case OutputQualityDto.Fax:
                    return OutputQuality.Fax;
                case OutputQualityDto.High:
                    return OutputQuality.High;
                case OutputQualityDto.Normal:
                    return OutputQuality.Normal;
                case OutputQualityDto.Photographic:
                    return OutputQuality.Photographic;
                case OutputQualityDto.Text:
                    return OutputQuality.Text;
                case OutputQualityDto.Unknown:
                    return OutputQuality.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown OutputQuality value: {outputQuality}");
            }
        }

        private static OutputColor? ToPrintTicketOutputColor(OutputColorDto outputColor)
        {
            switch (outputColor)
            {
                case OutputColorDto.Color:
                    return OutputColor.Color;
                case OutputColorDto.Grayscale:
                    return OutputColor.Grayscale;
                case OutputColorDto.Monochrome:
                    return OutputColor.Monochrome;
                case OutputColorDto.Unknown:
                    return OutputColor.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown OutputColor value: {outputColor}");
            }
        }

        private static InputBin? ToPrintTicketInputBin(InputBinDto inputBin)
        {
            switch (inputBin)
            {
                case InputBinDto.AutoSelect:
                    return InputBin.AutoSelect;
                case InputBinDto.AutoSheetFeeder:
                    return InputBin.AutoSheetFeeder;
                case InputBinDto.Cassette:
                    return InputBin.Cassette;
                case InputBinDto.Manual:
                    return InputBin.Manual;
                case InputBinDto.Tractor:
                    return InputBin.Tractor;
                case InputBinDto.Unknown:
                    return InputBin.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown InputBin value: {inputBin}");
            }
        }

        private static Duplexing? ToPrintTicketDuplixing(DuplexingDto duplexing)
        {
            switch (duplexing)
            {
                case DuplexingDto.OneSided:
                    return Duplexing.OneSided;
                case DuplexingDto.TwoSidedLongEdge:
                    return Duplexing.TwoSidedLongEdge;
                case DuplexingDto.TwoSidedShortEdge:
                    return Duplexing.TwoSidedShortEdge;
                case DuplexingDto.Unknown:
                    return Duplexing.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown Duplexing value: {duplexing}");
            }
        }

        private static DeviceFontSubstitution? ToPrintTicketDeviceFontSubstitution(DeviceFontSubstitutionDto deviceFontSubstitution)
        {
            switch (deviceFontSubstitution)
            {
                case DeviceFontSubstitutionDto.On:
                    return DeviceFontSubstitution.On;
                case DeviceFontSubstitutionDto.Off:
                    return DeviceFontSubstitution.Off;
                case DeviceFontSubstitutionDto.Unknown:
                    return DeviceFontSubstitution.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown DeviceFontSubstitutionDto value: {deviceFontSubstitution}");
            }
        }

        private static Collation? ToPrintTicketCollation(CollationDto collation)
        {
            switch (collation)
            {
                case CollationDto.Collated:
                    return Collation.Collated;
                case CollationDto.Uncollated:
                    return Collation.Uncollated;
                case CollationDto.Unknown:
                    return Collation.Unknown;
                default:
                    throw new ConversionFailedException($"Invalid or unknown CollationDto value: {collation}");
            }
        }
    }
}
