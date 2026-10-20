using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

[ApiController]
[Route("api/[controller]")]
[Tags("topics")]
public class TopicsController : ControllerBase
{
    private readonly IServiceBusService _sbusService;

    public TopicsController(IServiceBusService sbusService)
    {
        _sbusService = sbusService;
    }

    [HttpPost("{topicName}")]
    public async Task<IActionResult> SendMessageAsync(
        [FromRoute] string topicName,
        [FromBody] object message)
    {
        await _sbusService.SendMessageAsync(topicName, message);
        return Ok();
    }

    /// <summary>
    /// Reads messages from the subscription without removing them.
    /// </summary>
    [HttpGet("{topicName}/{subscription}/peek")]
    public async Task<IActionResult> PeekMessagesAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default)
        => Ok(await _sbusService.PeekMessagesAsync(topicName, subscription, maxMessages, cancellation));

    /// <summary>
    /// Reads messages from the subscription and removes them.
    /// </summary>
    [HttpGet("{topicName}/{subscription}")]
    public async Task<IActionResult> GetMessagesAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default)
        => Ok(await _sbusService.ReceiveMessagesAsync(topicName, subscription, maxMessages, cancellation));

    /// <summary>
    /// Removes every active message from the subscription.
    /// </summary>
    [HttpDelete("{topicName}/{subscription}/messages")]
    public async Task<IActionResult> PurgeMessagesAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscription,
        CancellationToken cancellation = default)
        => Ok(new { Purged = await _sbusService.PurgeMessagesAsync(topicName, subscription, cancellation) });
}
