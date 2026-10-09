using System.Xml.Serialization;

namespace Infrastructure.Services.Market.BigData.EntityBuilders;

[XmlRoot("Transaction")]
public class UsageTransactionXml
{
    public string? SupplierDUNS { get; set; }
    public string? UtilityDUNS { get; set; }
    public string? TransactionSet { get; set; }
    public string? TransactionSubSet { get; set; }
    public string? State { get; set; }
    public string? Commodity { get; set; }

    public UsageXml Usage { get; set; } = new();
}

public class UsageXml
{
    public string? PurposeCode { get; set; }
    public string? ReportType { get; set; }
    public string? FinalIndicator { get; set; }
    public string? TransactionReferenceNumber { get; set; }
    public string? OriginalTransactionNumber { get; set; }
    public string? UtilityName { get; set; }
    public string? EGSName { get; set; }
    public string? CreateDateTime { get; set; }
    public string? TimestampReceived { get; set; }
    public string? UtilityAccountNumber { get; set; }
    public string? TranNr814 { get; set; }

    [XmlArray("UsageMeterList")]
    [XmlArrayItem("UsageMeter")]
    public List<UsageMeterXml> UsageMeters { get; set; } = [];
}

public class UsageMeterXml
{
    public string? UsageType { get; set; }
    public string ServicePeriodBeginDate { get; set; } = string.Empty;
    public string ServicePeriodEndDate { get; set; } = string.Empty;
    public string? MeterIdentifier { get; set; }
    public string? MeterExchangeDate { get; set; }
    public string? MeterUOM { get; set; }
    public string? IntervalType { get; set; }
    public string? MeterRole { get; set; }
    public string? MeterUnmeterFlag { get; set; }
    public string? SwitchDate { get; set; }

    [XmlArray("UsageQuantityList")]
    [XmlArrayItem("UsageQuantity")]
    public List<UsageQuantityXml> UsageQuantities { get; set; } = [];
}

public class UsageQuantityXml
{
    public string? Qualifier { get; set; }
    public double? Quantity { get; set; }

    [XmlArray("UsageReadList")]
    [XmlArrayItem("UsageRead")]
    public List<UsageReadXml> UsageReads { get; set; } = [];
}

public class UsageReadXml
{
    public double ReadConsumption { get; set; }
    public string ReadUOM { get; set; } = string.Empty;
    public string? ReadFlag { get; set; }
    public string? TOU { get; set; }
    public double? StartMeterRead { get; set; }
    public double? EndMeterRead { get; set; }
    public double? Multiplier { get; set; }
}
