namespace Infrastructure.Data.Queries.Common;

/// <summary>
/// Maps DTO property names to entity property names for search and ordering,
/// acting as an allow-list so clients can only sort/filter by exposed columns.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IQueryPropertyMapper<TEntity> where TEntity : class
{
    /// <summary>
    /// Maps a DTO property name to the corresponding entity property name.
    /// </summary>
    /// <param name="dtoPropertyName">The property name from the client (case-insensitive)</param>
    /// <returns>The entity property name, or null if there is no mapping</returns>
    string? MapToEntityProperty(string dtoPropertyName);

    /// <summary>
    /// Gets the entity properties that are searchable via free-text search.
    /// </summary>
    string[] GetSearchableProperties();
}
