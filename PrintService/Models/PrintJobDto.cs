namespace PrintService.Models
{
    public record PrintJobInfoDto
    {
        public int? JobIdentifier { get; set; }
        public string? JobStatus { get; set; }
        public string? JobName { get; set; }
        public int? NumberOfPages { get; set; }
        public int? NumberOfPagesPrinted { get; set; }
        public int? PositionInPrintQueue { get; set; }
        public string? Priority { get; set; }
        public DateTime? TimeJobSubmitted { get; set; }
        public int? TimeSinceStartedPrinting { get; set; }
    }

    public record PrintTicketDto
    {
        public CollationDto? Collation { get; set; }
        public int? CopyCount { get; set; }
        public DeviceFontSubstitutionDto? DeviceFontSubstitution { get; set; }
        public DuplexingDto? Duplexing { get; set; }
        public InputBinDto? InputBin { get; set; }
        public OutputColorDto? OutputColor { get; set; }
        public OutputQualityDto? OutputQuality { get; set; }
        public PageBorderlessDto? PageBorderless { get; set; }
        public PageMediaSizeTicketDto? PageMediaSize { get; set; }
        public PageMediaTypeDto? PageMediaType { get; set; }
        public PageOrderDto? PageOrder { get; set; }
        public PageOrientationDto? PageOrientation { get; set; }
        public PageResolutionTicketDto? PageResolution { get; set; }
        public int? PageScalingFactor { get; set; }
        public int? PagesPerSheet { get; set; }
        public PagesPerSheetDirectionDto? PagesPerSheetDirection { get; set; }
        public PhotoPrintingIntentDto? PhotoPrintingIntent { get; set; }
        public StaplingDto? Stapling { get; set; }
        public TrueTypeFontModeDto? TrueTypeFontMode { get; set; }
    }


    public record PrintJobDto
    {
        public PrintTicketDto? Ticket { get; set; }
        public string? Name { get; set; }
        public string? DocumentPath { get; set; }
    }


    public record PageResolutionTicketDto
    {
        public PageQualitativeResolutionDto? Resolution { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
    }

    public enum PageQualitativeResolutionDto
    {
        Default,
        Draft,
        High,
        Normal,
        Other,
        Unknown,
    }

    public record PageMediaSizeTicketDto
    {
        public PageMediaSizeNameDto? Name { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
    }

    public enum TrueTypeFontModeDto
    {

        Automatic,
        DownloadAsNativeTrueTypeFont,
        DownloadAsOutlineFont,
        DownloadAsRasterFont,
        RenderAsBitmap,
        Unknown,
    }

    public enum CollationDto
    {
        Collated,
        Uncollated,
        Unknown
    }

    public enum DeviceFontSubstitutionDto
    {
        On,
        Off,
        Unknown,
    }

    public enum DuplexingDto
    {
        OneSided,
        TwoSidedLongEdge,
        TwoSidedShortEdge,
        Unknown,
    }

    public enum InputBinDto
    {
        AutoSelect,
        AutoSheetFeeder,
        Cassette,
        Manual,
        Tractor,
        Unknown,
    }

    public enum OutputColorDto
    {
        Color,
        Grayscale,
        Monochrome,
        Unknown,
    }

    public enum OutputQualityDto
    {
        Automatic,
        Draft,
        Fax,
        High,
        Normal,
        Photographic,
        Text,
        Unknown,
    }

    public enum PageBorderlessDto
    {

        None,
        Borderless,
        Unknown,
    }

    public enum PageMediaTypeDto
    {
        None,
        AutoSelect,
        Archival,
        BackPrintFilm,
        Bond,
        CardStock,
        Continuous,
        EnvelopePlain,
        EnvelopeWindow,
        Fabric,
        HighResolution,
        Label,
        MultiLayerForm,
        MultiPartForm,
        Photographic,
        PhotographicFilm,
        PhotographicGlossy,
        PhotographicHighGloss,
        PhotographicMatte,
        PhotographicSatin,
        PhotographicSemiGloss,
        Plain,
        Screen,
        ScreenPaged,
        Stationery,
        TabStockFull,
        TabStockPreCut,
        Transparency,
        TShirtTransfer,
        Unknown,
    }

    public enum PageOrderDto
    {

        Standard,
        Reverse,
        Unknown,
    }

    public enum PageOrientationDto
    {

        Landscape,
        Portrait,
        ReverseLandscape,
        ReversePortrait,
        Unknown,
    }

    public enum PagesPerSheetDirectionDto
    {
        TopRight,
        TopLeft,
        BottomLeft,
        BottomRight,
        LeftBottom,
        LeftTop,
        RightBottom,
        RightTop,
        Unknown,
    }

    public enum PhotoPrintingIntentDto
    {
        None,
        PhotoBest,
        PhotoDraft,
        PhotoStandard,
        Unknown,
    }

    public enum StaplingDto
    {

        None,
        StapleTopRight,
        StapleTopLeft,
        StapleDualTop,
        StapleDualRight,
        StapleDualLeft,
        StapleDualBottom,
        StapleBottomRight,
        StapleBottomLeft,
        SaddleStitch,
        Unknown,
    }

    public enum PageMediaSizeNameDto
    {
        ISOA0,
        ISOA1,
        ISOA10,
        ISOA2,
        ISOA3,
        ISOA3Rotated,
        ISOA3Extra,
        ISOA4,
        ISOA4Rotated,
        ISOA4Extra,
        ISOA5,
        ISOA5Rotated,
        ISOA5Extra,
        ISOA6,
        ISOA6Rotated,
        ISOA7,
        ISOA8,
        ISOA9,
        ISOB0,
        ISOB1,
        ISOB10,
        ISOB2,
        ISOB3,
        ISOB4,
        ISOB4Envelope,
        ISOB5Envelope,
        ISOB5Extra,
        ISOB7,
        ISOB8,
        ISOB9,
        ISOC0,
        ISOC1,
        ISOC10,
        ISOC2,
        ISOC3,
        ISOC3Envelope,
        ISOC4,
        ISOC4Envelope,
        ISOC5,
        ISOC5Envelope,
        ISOC6,
        ISOC6Envelope,
        ISOC6C5Envelope,
        ISOC7,
        ISOC8,
        ISOC9,
        ISODLEnvelope,
        ISODLEnvelopeRotated,
        ISOSRA3,
        JapanQuadrupleHagakiPostcard,
        JISB0,
        JISB1,
        JISB10,
        JISB2,
        JISB3,
        JISB4,
        JISB4Rotated,
        JISB5,
        JISB5Rotated,
        JISB6,
        JISB6Rotated,
        JISB7,
        JISB8,
        JISB9,
        JapanChou3Envelope,
        JapanChou3EnvelopeRotated,
        JapanChou4Envelope,
        JapanChou4EnvelopeRotated,
        JapanHagakiPostcard,
        JapanHagakiPostcardRotated,
        JapanKaku2Envelope,
        JapanKaku2EnvelopeRotated,
        JapanKaku3Envelope,
        JapanKaku3EnvelopeRotated,
        JapanYou4Envelope,
        NorthAmerica10x11,
        NorthAmerica10x14,
        NorthAmerica11x17,
        NorthAmerica9x11,
        NorthAmericaArchitectureASheet,
        NorthAmericaArchitectureBSheet,
        NorthAmericaArchitectureCSheet,
        NorthAmericaArchitectureDSheet,
        NorthAmericaArchitectureESheet,
        NorthAmericaCSheet,
        NorthAmericaDSheet,
        NorthAmericaESheet,
        NorthAmericaExecutive,
        NorthAmericaGermanLegalFanfold,
        NorthAmericaGermanStandardFanfold,
        NorthAmericaLegal,
        NorthAmericaLegalExtra,
        NorthAmericaLetter,
        NorthAmericaLetterRotated,
        NorthAmericaLetterExtra,
        NorthAmericaLetterPlus,
        NorthAmericaMonarchEnvelope,
        NorthAmericaNote,
        NorthAmericaNumber10Envelope,
        NorthAmericaNumber10EnvelopeRotated,
        NorthAmericaNumber9Envelope,
        NorthAmericaNumber11Envelope,
        NorthAmericaNumber12Envelope,
        NorthAmericaNumber14Envelope,
        NorthAmericaPersonalEnvelope,
        NorthAmericaQuarto,
        NorthAmericaStatement,
        NorthAmericaSuperA,
        NorthAmericaSuperB,
        NorthAmericaTabloid,
        NorthAmericaTabloidExtra,
        OtherMetricA4Plus,
        OtherMetricA3Plus,
        OtherMetricFolio,
        OtherMetricInviteEnvelope,
        OtherMetricItalianEnvelope,
        PRC1Envelope,
        PRC1EnvelopeRotated,
        PRC10Envelope,
        PRC10EnvelopeRotated,
        PRC16K,
        PRC16KRotated,
        PRC2Envelope,
        PRC2EnvelopeRotated,
        PRC32K,
        PRC32KRotated,
        PRC32KBig,
        PRC3Envelope,
        PRC3EnvelopeRotated,
        PRC4Envelope,
        PRC4EnvelopeRotated,
        PRC5Envelope,
        PRC5EnvelopeRotated,
        PRC6Envelope,
        PRC6EnvelopeRotated,
        PRC7Envelope,
        PRC7EnvelopeRotated,
        PRC8Envelope,
        PRC8EnvelopeRotated,
        PRC9Envelope,
        PRC9EnvelopeRotated,
        Roll04Inch,
        Roll06Inch,
        Roll08Inch,
        Roll12Inch,
        Roll15Inch,
        Roll18Inch,
        Roll22Inch,
        Roll24Inch,
        Roll30Inch,
        Roll36Inch,
        Roll54Inch,
        JapanDoubleHagakiPostcard,
        JapanDoubleHagakiPostcardRotated,
        JapanLPhoto,
        Japan2LPhoto,
        JapanYou1Envelope,
        JapanYou2Envelope,
        JapanYou3Envelope,
        JapanYou4EnvelopeRotated,
        JapanYou6Envelope,
        JapanYou6EnvelopeRotated,
        NorthAmerica4x6,
        NorthAmerica4x8,
        NorthAmerica5x7,
        NorthAmerica8x10,
        NorthAmerica10x12,
        NorthAmerica14x17,
        BusinessCard,
        CreditCard,
        Unknown,
    }
}
