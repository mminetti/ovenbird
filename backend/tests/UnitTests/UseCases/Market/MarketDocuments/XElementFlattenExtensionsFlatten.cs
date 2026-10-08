using System.Xml.Linq;
using UseCases.Market.MarketDocuments.Import.BigData.Readers;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class XElementFlattenExtensionsFlatten
{
    [Fact]
    public void DropsListWrappersAndIndexesRepeatedElements()
    {
        var usage = XElement.Parse("""
            <Usage>
              <PurposeCode>00</PurposeCode>
              <ReportType>DD</ReportType>
              <UsageMeterList>
                <UsageMeter>
                  <UsageType>DET</UsageType>
                  <MeterIdentifier>200238219</MeterIdentifier>
                  <UsageQuantityList>
                    <UsageQuantity>
                      <Qualifier>QD</Qualifier>
                      <Quantity>666</Quantity>
                      <UsageReadList>
                        <UsageRead>
                          <ReadConsumption>666</ReadConsumption>
                          <ReadUOM>KH</ReadUOM>
                        </UsageRead>
                      </UsageReadList>
                    </UsageQuantity>
                  </UsageQuantityList>
                </UsageMeter>
                <UsageMeter>
                  <UsageType>SUM</UsageType>
                  <UsageQuantityList>
                    <UsageQuantity>
                      <Qualifier>QD</Qualifier>
                      <Quantity>666</Quantity>
                      <UsageReadList>
                        <UsageRead>
                          <ReadConsumption>666</ReadConsumption>
                          <ReadUOM>KH</ReadUOM>
                        </UsageRead>
                      </UsageReadList>
                    </UsageQuantity>
                  </UsageQuantityList>
                </UsageMeter>
              </UsageMeterList>
            </Usage>
            """);

        var fields = usage.Flatten("Usage");

        fields.ShouldBe(
        [
            ("Usage.PurposeCode", "00"),
            ("Usage.ReportType", "DD"),
            ("Usage.UsageMeter[0].UsageType", "DET"),
            ("Usage.UsageMeter[0].MeterIdentifier", "200238219"),
            ("Usage.UsageMeter[0].UsageQuantity[0].Qualifier", "QD"),
            ("Usage.UsageMeter[0].UsageQuantity[0].Quantity", "666"),
            ("Usage.UsageMeter[0].UsageQuantity[0].UsageRead[0].ReadConsumption", "666"),
            ("Usage.UsageMeter[0].UsageQuantity[0].UsageRead[0].ReadUOM", "KH"),
            ("Usage.UsageMeter[1].UsageType", "SUM"),
            ("Usage.UsageMeter[1].UsageQuantity[0].Qualifier", "QD"),
            ("Usage.UsageMeter[1].UsageQuantity[0].Quantity", "666"),
            ("Usage.UsageMeter[1].UsageQuantity[0].UsageRead[0].ReadConsumption", "666"),
            ("Usage.UsageMeter[1].UsageQuantity[0].UsageRead[0].ReadUOM", "KH"),
        ]);
    }

    [Fact]
    public void FlattensLeafElementWithoutIndexingUniqueSiblings()
    {
        var usage = XElement.Parse("<Usage><PurposeCode>00</PurposeCode></Usage>");

        var fields = usage.Flatten("Usage");

        fields.ShouldBe([("Usage.PurposeCode", "00")]);
    }
}
