namespace Worker.Jobs;

public class MarketDocumentImportOptions
{
    public const string SectionName = "MarketDocumentImport";

    public string RemoteFilePath { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string CronSchedule { get; set; } = "0 0 2 * * ?";
}
