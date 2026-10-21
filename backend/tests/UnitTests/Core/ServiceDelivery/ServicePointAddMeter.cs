using Core.ServiceDelivery;

namespace UnitTests.Core.ServiceDelivery;

public class ServicePointAddMeter
{
    [Fact]
    public void AddsNewMeterAsActive()
    {
        var servicePoint = new ServicePoint { Id = 7 };

        var meter = servicePoint.AddMeter("M1");

        meter.Identifier.ShouldBe("M1");
        meter.ServicePointId.ShouldBe(servicePoint.Id);
        meter.IsActive.ShouldBeTrue();
        servicePoint.Meters.ShouldHaveSingleItem().ShouldBe(meter);
    }

    [Fact]
    public void DeactivatesExistingMetersWhenAddingANewOne()
    {
        var servicePoint = new ServicePoint { Id = 7 };
        var existing = new Meter { Identifier = "OLD", IsActive = true };
        servicePoint.Meters.Add(existing);

        servicePoint.AddMeter("NEW");

        existing.IsActive.ShouldBeFalse();
        servicePoint.GetMeter("NEW")!.IsActive.ShouldBeTrue();
        servicePoint.Meters.Count.ShouldBe(2);
    }

    [Fact]
    public void ReturnsExistingMeterWithoutDeactivatingAnythingWhenAlreadyPresent()
    {
        var servicePoint = new ServicePoint { Id = 7 };
        var existing = new Meter { Identifier = "M1", IsActive = true };
        var other = new Meter { Identifier = "OTHER", IsActive = true };
        servicePoint.Meters.Add(existing);
        servicePoint.Meters.Add(other);

        var meter = servicePoint.AddMeter("M1");

        meter.ShouldBe(existing);
        existing.IsActive.ShouldBeTrue();
        other.IsActive.ShouldBeTrue();
        servicePoint.Meters.Count.ShouldBe(2);
    }
}
