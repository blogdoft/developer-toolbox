using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage;

[ApiController]
[Route("api/storage/queues")]
[Tags("storage-queues")]
public class QueuesStorageController : ControllerBase
{
    private readonly IQueueStorageService _service;

    public QueuesStorageController(IQueueStorageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetQueuesAsync(CancellationToken cancellation)
        => Ok(await _service.GetQueuesAsync(cancellation));

    [HttpPost("{queue}")]
    public async Task<IActionResult> CreateQueueAsync(string queue, CancellationToken cancellation)
    {
        await _service.CreateQueueAsync(queue, cancellation);
        return Created();
    }

    [HttpDelete("{queue}")]
    public async Task<IActionResult> DeleteQueueAsync(string queue, CancellationToken cancellation)
    {
        await _service.DeleteQueueAsync(queue, cancellation);
        return NoContent();
    }

    /// <summary>
    /// Reads messages without removing them nor changing their visibility.
    /// </summary>
    [HttpGet("{queue}/messages")]
    public async Task<IActionResult> PeekMessagesAsync(
        string queue,
        [FromQuery] int maxMessages = 10,
        CancellationToken cancellation = default)
        => Ok(await _service.PeekMessagesAsync(queue, maxMessages, cancellation));

    [HttpPost("{queue}/messages")]
    public async Task<IActionResult> SendMessageAsync(
        string queue,
        [FromBody] SendQueueMessageRequest request,
        CancellationToken cancellation)
    {
        await _service.SendMessageAsync(queue, request.Text, cancellation);
        return Created();
    }

    [HttpDelete("{queue}/messages")]
    public async Task<IActionResult> ClearMessagesAsync(string queue, CancellationToken cancellation)
    {
        await _service.ClearMessagesAsync(queue, cancellation);
        return NoContent();
    }
}
