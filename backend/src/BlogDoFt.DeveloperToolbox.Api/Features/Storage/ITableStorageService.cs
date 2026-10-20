namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage;

/// <summary>
/// Manages storage tables and their entities. Entities are exposed as property bags,
/// since tables are schemaless.
/// </summary>
public interface ITableStorageService
{
    Task<IReadOnlyList<string>> GetTablesAsync(CancellationToken cancellation = default);

    Task CreateTableAsync(string table, CancellationToken cancellation = default);

    Task DeleteTableAsync(string table, CancellationToken cancellation = default);

    /// <param name="filter">OData filter, e.g. <c>PartitionKey eq 'a'</c>.</param>
    Task<IReadOnlyList<IDictionary<string, object?>>> GetEntitiesAsync(
        string table,
        string? filter,
        int maxEntities,
        CancellationToken cancellation = default);

    /// <summary>
    /// Inserts the entity, or replaces it when the same keys already exist.
    /// </summary>
    Task UpsertEntityAsync(
        string table,
        IDictionary<string, object?> entity,
        CancellationToken cancellation = default);

    Task DeleteEntityAsync(
        string table,
        string partitionKey,
        string rowKey,
        CancellationToken cancellation = default);
}
