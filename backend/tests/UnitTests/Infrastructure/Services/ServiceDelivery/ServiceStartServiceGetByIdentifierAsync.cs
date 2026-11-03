using Ardalis.Result;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Infrastructure.Services.ServiceDelivery;

namespace UnitTests.Infrastructure.Services.ServiceDelivery;

public class ServiceStartServiceGetByIdentifierAsync
{
    private readonly IReadRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IReadRepository<ServiceStartItem>>();
    private readonly ServiceStartService _service;

    public ServiceStartServiceGetByIdentifierAsync()
    {
        _service = new ServiceStartService(_serviceStartItemRepository);
    }

    [Fact]
    public async Task ReturnsInvalidWhenIdentifierIsMissing()
    {
        var result = await _service.GetByIdentifierAsync(null, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldHaveSingleItem().Identifier.ShouldBe(nameof(ServiceStart));
    }

    [Fact]
    public async Task ReturnsInvalidWhenServiceStartItemIsNotFound()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStartItem?)null);

        var result = await _service.GetByIdentifierAsync("some-identifier", CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldHaveSingleItem().Identifier.ShouldBe(nameof(ServiceStart));
    }

    [Fact]
    public async Task ReturnsServiceStartWhenServiceStartItemIsFound()
    {
        var serviceStart = new ServiceStart { Id = 99 };
        var serviceStartItem = new ServiceStartItem
        {
            Id = 1,
            ServiceStartId = serviceStart.Id,
            Identifier = "some-identifier",
            ServiceStart = serviceStart,
        };

        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(serviceStartItem);

        var result = await _service.GetByIdentifierAsync("some-identifier", CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(serviceStart);
    }
}
