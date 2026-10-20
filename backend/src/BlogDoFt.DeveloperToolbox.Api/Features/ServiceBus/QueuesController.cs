using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

[ApiController]
[Route("api/[controller]")]
[Tags("queues")]
public class QueuesController : ControllerBase
{
    private readonly IServiceBusService _sbusService;

    public QueuesController(IServiceBusService sbusService)
    {
        _sbusService = sbusService;
    }

    [HttpPost("{queueName}")]
    public async Task<IActionResult> SendMessageAsync(
        [FromRoute] string queueName,
        [FromBody] object message)
    {
        await _sbusService.SendMessageAsync(queueName, message);
        return Ok();
    }

    /// <summary>
    /// Reads messages from the queue without removing them.
    /// </summary>
    [HttpGet("{queueName}/peek")]
    public async Task<IActionResult> PeekMessagesAsync(
        [FromRoute] string queueName,
        int maxMessages = 10,
        CancellationToken cancellation = default)
        => Ok(await _sbusService.PeekMessagesAsync(queueName, null, maxMessages, cancellation));

    /// <summary>
    /// Reads messages from the queue and removes them.
    /// </summary>
    [HttpGet("{queueName}")]
    public async Task<IActionResult> GetMessagesAsync(
        [FromRoute] string queueName,
        int maxMessages = 10,
        CancellationToken cancellation = default)
        => Ok(await _sbusService.ReceiveMessagesAsync(queueName, null, maxMessages, cancellation));

    /// <summary>
    /// Removes every active message from the queue.
    /// </summary>
    [HttpDelete("{queueName}/messages")]
    public async Task<IActionResult> PurgeMessagesAsync(
        [FromRoute] string queueName,
        CancellationToken cancellation = default)
        => Ok(new { Purged = await _sbusService.PurgeMessagesAsync(queueName, null, cancellation) });
}
