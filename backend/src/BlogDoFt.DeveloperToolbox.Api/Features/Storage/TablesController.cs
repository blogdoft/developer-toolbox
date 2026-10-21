using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage;

[ApiController]
[Route("api/storage/tables")]
[Tags("storage-tables")]
public class TablesController : ControllerBase
{
    private readonly ITableStorageService _service;

    public TablesController(ITableStorageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetTablesAsync(CancellationToken cancellation)
        => Ok(await _service.GetTablesAsync(cancellation));

    [HttpPost("{table}")]
    public async Task<IActionResult> CreateTableAsync(string table, CancellationToken cancellation)
    {
        await _service.CreateTableAsync(table, cancellation);
        return Created();
    }

    [HttpDelete("{table}")]
    public async Task<IActionResult> DeleteTableAsync(string table, CancellationToken cancellation)
    {
        await _service.DeleteTableAsync(table, cancellation);
        return NoContent();
    }

    /// <param name="filter">OData filter, e.g. <c>PartitionKey eq 'a'</c>.</param>
    [HttpGet("{table}/entities")]
    public async Task<IActionResult> GetEntitiesAsync(
        string table,
        [FromQuery] string? filter,
        [FromQuery] int top = 100,
        CancellationToken cancellation = default)
        => Ok(await _service.GetEntitiesAsync(table, filter, Math.Clamp(top, 1, 1000), cancellation));

    /// <summary>
    /// Inserts or replaces an entity. <c>PartitionKey</c> and <c>RowKey</c> are required.
    /// </summary>
    [HttpPut("{table}/entities")]
    public async Task<IActionResult> UpsertEntityAsync(
        string table,
        [FromBody] Dictionary<string, JsonElement> entity,
        CancellationToken cancellation)
    {
        if (!entity.ContainsKey("PartitionKey") || !entity.ContainsKey("RowKey"))
        {
            return Problem(
                detail: "PartitionKey and RowKey are required.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await _service.UpsertEntityAsync(
            table,
            entity.ToDictionary(pair => pair.Key, pair => (object?)pair.Value),
            cancellation);

        return NoContent();
    }

    [HttpDelete("{table}/entities")]
    public async Task<IActionResult> DeleteEntityAsync(
        string table,
        [FromQuery] string partitionKey,
        [FromQuery] string rowKey,
        CancellationToken cancellation)
    {
        await _service.DeleteEntityAsync(table, partitionKey, rowKey, cancellation);
        return NoContent();
    }
}
