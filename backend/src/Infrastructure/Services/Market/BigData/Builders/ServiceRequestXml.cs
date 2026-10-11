using System.Xml.Serialization;

namespace Infrastructure.Services.Market.BigData.Builders;

[XmlRoot("Transaction")]
public class ServiceRequestXml
{
    public string SupplierDUNS { get; set; } = string.Empty;
    public string UtilityDUNS { get; set; } = string.Empty;
    public string TransactionSet { get; set; } = string.Empty;
    public string TransactionSubSet { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Commodity { get; set; } = string.Empty;

    public AccountMaintXml AccountMaint { get; set; } = new();
}

public class AccountMaintXml
{
    public string? ResponseRequestFlag { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public string? ProcessTrackingNumber { get; set; }
    public string? UtilityName { get; set; }
    public string? ESPName { get; set; }
    public string? DateStampCreated { get; set; }
    public string? DateStampReceived { get; set; }
    public string? TimeStampReceived { get; set; }

    [XmlArray("NameAddressList")]
    [XmlArrayItem("NameAddress")]
    public List<NameAddressXml> NameAddresses { get; set; } = [];

    [XmlArray("ServiceList")]
    [XmlArrayItem("Service")]
    public List<ServiceXml> Services { get; set; } = [];
}

public class NameAddressXml
{
    public string? EntityType { get; set; }
    public string? EntityName { get; set; }
    public string? EntityAddress1 { get; set; }
    public string? EntityCity { get; set; }
    public string? EntityState { get; set; }
    public string? EntityPostalCode { get; set; }
    public string? EntityCounty { get; set; }
}

public class ServiceXml
{
    public string? ProductID { get; set; }
    public string? ServiceRequested { get; set; }
    public string? SecondServiceRequested { get; set; }
    public string? ThirdServiceRequested { get; set; }
    public string? LineTrackNumber { get; set; }
    public string? ActionCode { get; set; }
    public string UtilityAccountNumber { get; set; } = string.Empty;
    public string? SwitchHoldInd { get; set; }
    public string? StdPointLocationCode { get; set; }
    public string? DistributionLoss { get; set; }
    public string? PremiseType { get; set; }
    public string? SpecialNeedsInfo { get; set; }
    public string? Indicator814 { get; set; }
    public string? PermitIndicator { get; set; }
    public string? RejectCode { get; set; }
    public string? RejectReason { get; set; }

    [XmlArray("ServiceChangeReasonList")]
    [XmlArrayItem("ServiceChangeReason")]
    public List<ServiceChangeReasonXml> ServiceChangeReasons { get; set; } = [];

    [XmlArray("ServiceDateList")]
    [XmlArrayItem("ServiceDate")]
    public List<ServiceDateXml> ServiceDates { get; set; } = [];

    [XmlArray("ServiceStatusList")]
    [XmlArrayItem("ServiceStatus")]
    public List<ServiceStatusXml> ServiceStatuses { get; set; } = [];

    [XmlArray("ServiceMeterList")]
    [XmlArrayItem("ServiceMeter")]
    public List<ServiceMeterXml> ServiceMeters { get; set; } = [];
}

public class ServiceChangeReasonXml
{
    public string? ChangeReason { get; set; }
}

public class ServiceDateXml
{
    public string? CustomerAgreementDate { get; set; }
    public string? MoveOutDate { get; set; }
    public string? ServiceEndDate { get; set; }
}

public class ServiceStatusXml
{
    public string? StatusCode { get; set; }
}

public class ServiceMeterXml
{
    public string? MeterNumber { get; set; }
    public string? MeterAction { get; set; }
    public string? BillDataAndIntervalType { get; set; }
    public string? LDCRateClass { get; set; }
    public string? LoadProfileCode { get; set; }
    public string? OwnerMeterCycle { get; set; }
    public string? AdvanceMeterInfo { get; set; }
    public string? MeterConfiguration { get; set; }
    public string? MeterConfigDesc { get; set; }

    [XmlArray("ServiceMeterTOUList")]
    [XmlArrayItem("ServiceMeterTOU")]
    public List<ServiceMeterTOUXml> ServiceMeterTOUs { get; set; } = [];
}

public class ServiceMeterTOUXml
{
    public string? MeterType { get; set; }
    public double? MeterMultiplier { get; set; }
    public double? NumberOfDials { get; set; }
    public string? TimeOfUse1 { get; set; }
}
