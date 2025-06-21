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

    [HttpGet("{topicName}/{subscription}")]
    public async Task<IActionResult> GetMessagesAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default)
    {
        var msgs = await _sbusService.TopicReceiveMessagesAsync(
            topicName,
            subscription,
            maxMessages,
            cancellation);

        return Ok(msgs);
    }
}
