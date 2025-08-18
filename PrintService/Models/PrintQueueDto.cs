namespace PrintService.Models
{
    public record PrintQueueDto
    {
        public string? FullName { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }

    public record PrintCapabilitiesDto
    {
        public List<string>? Collation { get; set; }
        public List<string>? DeviceFontSubstitution { get; set; }
        public List<string>? Duplexing { get; set; }
        public List<string>? InputBin { get; set; }
        public int? MaxCopyCount { get; set; }
        public double? OrientedPageMediaHeight { get; set; }
        public double? OrientedPageMediaWidth { get; set; }
        public List<string>? OutputColor { get; set; }
        public List<string>? OutputQuality { get; set; }
        public List<string>? PageBorderless { get; set; }
        public PageImageableAreaDto? PageImageableArea { get; set; }
        public List<PageMediaSizeDto>? PageMediaSize { get; set; }
        public List<string>? PageMediaType { get; set; }
        public List<string>? PageOrder { get; set; }
        public List<string>? PageOrientation { get; set; }
        public List<PageResolutionDto>? PageResolution { get; set; }
        public PageScalingFactorRangeDto? PageScalingFactorRange { get; set; }
        public List<string>? PagesPerSheet { get; set; }
        public List<string>? PagesPerSheetDirection { get; set; }
        public List<string>? PhotoPrintingIntent { get; set; }
        public List<string>? Stapling { get; set; }
        public List<string>? TrueTypeFontMode { get; set; }
    }

    public record PageImageableAreaDto
    {
        public double? ExtentHeight { get; set; }
        public double? ExtentWidth { get; set; }
        public double? OriginHeight { get; set; }
        public double? OriginWidth { get; set; }
    }

    public record PageMediaSizeDto
    {
        public string? SizeName { get; set; }
        public string? Height { get; set; }
        public string? Width { get; set; }
    }

    public record PageResolutionDto
    {
        public string? Resolution { get; set; }
        public string? X { get; set; }
        public string? Y { get; set; }
    }

    public record PageScalingFactorRangeDto
    {
        public int? MaximumScale { get; set; }
        public int? MinimumScale { get; set; }
    }
}
