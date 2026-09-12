using Core.Market;
using Core.Security;
using Core.Shared;
using UseCases.DataLists;
using UseCases.DataLists.Get;

namespace Infrastructure.Data.Queries.DataLists;

/// <summary>
/// Query service for retrieving reference data for UI dropdowns. The request's
/// <see cref="DataListType" /> is validated at the Web boundary (GetDataListValidator),
/// so an unmapped type here indicates a new enum member that was never wired up.
/// </summary>
public class GetDataListQueryService(ReadDbContext db) : IGetDataListQueryService
{
    public async Task<IReadOnlyList<ValuePairDto>> GetListAsync(DataListType type, CancellationToken ct)
    {
        return type switch
        {
            DataListType.MarketDocumentDirections => await db.Set<MarketDocumentDirection>()
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new ValuePairDto(x.Id.ToString(), x.Name))
                .ToListAsync(ct),

            DataListType.MarketDocumentStatuses => await db.Set<MarketDocumentStatus>()
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new ValuePairDto(x.Id.ToString(), x.Name))
                .ToListAsync(ct),

            DataListType.Markets => await db.Set<Market>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new ValuePairDto(x.Id.ToString(), x.Name))
                .ToListAsync(ct),

            DataListType.ConnectorTypes => await db.Set<ConnectorType>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new ValuePairDto(x.Id.ToString(), x.Name))
                .ToListAsync(ct),

            DataListType.ConnectorImplementations => await db.Set<ConnectorImplementation>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new ValuePairDto(x.Id.ToString(), x.Name))
                .ToListAsync(ct),

            DataListType.ConfigurationTypes => await db.Set<ConfigurationType>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new ValuePairDto(x.Id.ToString(), x.Name))
                .ToListAsync(ct),

            DataListType.Roles => await db.Set<Role>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new ValuePairDto(x.Id.ToString(), x.Name))
                .ToListAsync(ct),

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown data list type")
        };
    }
}
