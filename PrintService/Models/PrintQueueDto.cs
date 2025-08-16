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
        public Dictionary<string, double?>? PageImageableArea { get; set; }
        public List<string>? PageMediaSize { get; set; }
        public List<string>? PageMediaType { get; set; }
        public List<string>? PageOrder { get; set; }
        public List<string>? PageOrientation { get; set; }
        public List<string>? PageResolution { get; set; }
        public Dictionary<string, int?>? PageScalingFactorRange { get; set; }
        public List<string>? PagesPerSheet { get; set; }
        public List<string>? PagesPerSheetDirection { get; set; }
        public List<string>? PhotoPrintingIntent { get; set; }
        public List<string>? Stapling { get; set; }
        public List<string>? TrueTypeFontMode { get; set; }
    }

}
