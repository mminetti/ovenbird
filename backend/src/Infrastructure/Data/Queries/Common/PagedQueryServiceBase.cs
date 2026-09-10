using System.Linq.Expressions;
using Infrastructure.Data.Extensions;
using UseCases.Common;

namespace Infrastructure.Data.Queries.Common;

/// <summary>
/// Base class for paginated query services with search, ordering, and pagination.
/// </summary>
/// <typeparam name="TEntity">The entity type from the database</typeparam>
/// <typeparam name="TDto">The DTO type returned to clients</typeparam>
public abstract class PagedQueryServiceBase<TEntity, TDto>(IQueryPropertyMapper<TEntity> propertyMapper)
    where TEntity : class
    where TDto : class
{
    /// <summary>
    /// Gets the queryable source for the entity type.
    /// </summary>
    protected abstract IQueryable<TEntity> GetQuery();

    /// <summary>
    /// Gets the projection expression from entity to DTO.
    /// </summary>
    protected abstract Expression<Func<TEntity, TDto>> GetProjection();

    /// <summary>
    /// Gets the default ordering property name (entity property), used when no
    /// client order-by is given or it doesn't map to an allowed property.
    /// </summary>
    protected virtual string GetDefaultOrderBy() => "Id";

    public async Task<ItemPagedResult<TDto>> ListAsync(
        int page,
        int perPage,
        string? search,
        string? orderBy,
        CancellationToken ct)
    {
        var query = GetQuery();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.WhereContainsText(search, propertyMapper.GetSearchableProperties());
        }

        int totalCount = await query.CountAsync(ct);

        string? entityOrderBy = null;
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            var parts = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var mappedProperty = propertyMapper.MapToEntityProperty(parts[0]);

            if (mappedProperty is not null)
            {
                entityOrderBy = parts.Length > 1 ? $"{mappedProperty} {parts[1]}" : mappedProperty;
            }
        }

        var defaultOrderBy = GetDefaultOrderBy();
        query = string.IsNullOrWhiteSpace(entityOrderBy)
            ? query.OrderByDynamic(defaultOrderBy)
            : query.OrderByDynamic(entityOrderBy).ThenBy(x => EF.Property<object>(x, defaultOrderBy));

        var items = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(GetProjection())
            .AsNoTracking()
            .ToListAsync(ct);

        int totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

        return new ItemPagedResult<TDto>(items, page, perPage, totalCount, totalPages);
    }
}
