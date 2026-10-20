using Azure.Data.Tables;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage.Impl;

public class TableStorageService : ITableStorageService
{
    private const string ETagKey = "odata.etag";

    private readonly TableServiceClient _client;

    public TableStorageService(TableServiceClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<string>> GetTablesAsync(CancellationToken cancellation = default)
    {
        var tables = new List<string>();
        await foreach (var table in _client.QueryAsync(cancellationToken: cancellation))
        {
            tables.Add(table.Name);
        }

        return tables;
    }

    public async Task CreateTableAsync(string table, CancellationToken cancellation = default)
        => await _client.CreateTableAsync(table, cancellation);

    public async Task DeleteTableAsync(string table, CancellationToken cancellation = default)
        => await _client.DeleteTableAsync(table, cancellation);

    public async Task<IReadOnlyList<IDictionary<string, object?>>> GetEntitiesAsync(
        string table,
        string? filter,
        int maxEntities,
        CancellationToken cancellation = default)
    {
        var entities = new List<IDictionary<string, object?>>();
        var query = _client.GetTableClient(table)
            .QueryAsync<TableEntity>(filter, maxPerPage: maxEntities, cancellationToken: cancellation);

        await foreach (var entity in query)
        {
            entities.Add(entity
                .Where(pair => pair.Key != ETagKey)
                .ToDictionary(pair => pair.Key, pair => (object?)pair.Value));

            if (entities.Count >= maxEntities)
            {
                break;
            }
        }

        return entities;
    }

    public async Task UpsertEntityAsync(
        string table,
        IDictionary<string, object?> entity,
        CancellationToken cancellation = default)
    {
        var tableEntity = new TableEntity();
        foreach (var (key, value) in entity)
        {
            if (key == nameof(TableEntity.Timestamp) || key == ETagKey)
            {
                continue;
            }

            var converted = Convert(value);
            if (converted is not null)
            {
                tableEntity[key] = converted;
            }
        }

        await _client.GetTableClient(table).UpsertEntityAsync(tableEntity, TableUpdateMode.Replace, cancellation);
    }

    public async Task DeleteEntityAsync(
        string table,
        string partitionKey,
        string rowKey,
        CancellationToken cancellation = default)
        => await _client.GetTableClient(table).DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellation);

    /// <summary>
    /// Maps the JSON values received from the API to the types supported by tables.
    /// Nulls are dropped, since tables have no null property.
    /// </summary>
    private static object? Convert(object? value) => value switch
    {
        JsonElement { ValueKind: JsonValueKind.String } json
            => json.TryGetDateTimeOffset(out var date) ? date : json.GetString(),
        JsonElement { ValueKind: JsonValueKind.True } => true,
        JsonElement { ValueKind: JsonValueKind.False } => false,
        JsonElement { ValueKind: JsonValueKind.Number } json => ConvertNumber(json),
        JsonElement { ValueKind: JsonValueKind.Null or JsonValueKind.Undefined } => null,
        JsonElement json => json.GetRawText(),
        _ => value,
    };

    private static object ConvertNumber(JsonElement json)
    {
        if (json.TryGetInt32(out var integer))
        {
            return integer;
        }

        return json.TryGetInt64(out var big) ? big : json.GetDouble();
    }
}
