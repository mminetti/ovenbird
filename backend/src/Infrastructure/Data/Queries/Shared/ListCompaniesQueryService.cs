using System.Linq.Expressions;
using Core.Shared;
using Infrastructure.Data.Queries.Common;
using UseCases.Companies;
using UseCases.Companies.List;

namespace Infrastructure.Data.Queries.Shared;

public class ListCompaniesQueryService(ReadDbContext db, IQueryPropertyMapper<Company> propertyMapper)
    : PagedQueryServiceBase<Company, CompanyDto>(propertyMapper), IListCompaniesQueryService
{
    protected override IQueryable<Company> GetQuery() => db.Company;

    protected override Expression<Func<Company, CompanyDto>> GetProjection() =>
        c => new CompanyDto(
            c.Id,
            c.Name,
            c.MarketId,
            c.Market.Name,
            c.TimeZoneId,
            c.LastModifiedAtUtc,
            c.LastModifiedBy);

    protected override string GetDefaultOrderBy() => nameof(Company.Id);
}
