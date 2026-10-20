using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

[ApiController]
[Route("api/[controller]")]
public class EntitiesController : ControllerBase
{
    private readonly INamespaceManager _namespace;

    public EntitiesController(INamespaceManager @namespace)
    {
        _namespace = @namespace;
    }

    [HttpGet]
    public async Task<IActionResult> ListEntitiesAsync(CancellationToken cancellation = default)
        => Ok(await _namespace.GetTopicsAsync(cancellation));

    [HttpGet("queues")]
    public async Task<IActionResult> ListQueuesAsync(CancellationToken cancellation = default)
        => Ok(await _namespace.GetQueuesAsync(cancellation));

    [HttpPost("topics/{topicName}")]
    public async Task<IActionResult> CreateTopicAsync(
        [FromRoute] string topicName,
        [FromBody] TopicSettings settings,
        CancellationToken cancellation = default)
        => Ok(await _namespace.CreateTopicAsync(topicName, settings, cancellation));

    [HttpPut("topics/{topicName}")]
    public async Task<IActionResult> UpdateTopicAsync(
        [FromRoute] string topicName,
        [FromBody] TopicSettings settings,
        CancellationToken cancellation = default)
    {
        await _namespace.UpdateTopicAsync(topicName, settings, cancellation);
        return NoContent();
    }

    [HttpDelete("topics/{topicName}")]
    public async Task<IActionResult> DeleteTopicAsync(
        [FromRoute] string topicName,
        CancellationToken cancellation = default)
    {
        await _namespace.DeleteTopicAsync(topicName, cancellation);
        return NoContent();
    }

    [HttpPost("topics/{topicName}/subscriptions/{subscriptionName}")]
    public async Task<IActionResult> CreateSubscriptionAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscriptionName,
        [FromBody] SubscriptionSettings settings,
        CancellationToken cancellation = default)
        => Ok(await _namespace.CreateSubscriptionAsync(topicName, subscriptionName, settings, cancellation));

    [HttpPut("topics/{topicName}/subscriptions/{subscriptionName}")]
    public async Task<IActionResult> UpdateSubscriptionAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscriptionName,
        [FromBody] SubscriptionSettings settings,
        CancellationToken cancellation = default)
    {
        await _namespace.UpdateSubscriptionAsync(topicName, subscriptionName, settings, cancellation);
        return NoContent();
    }

    [HttpDelete("topics/{topicName}/subscriptions/{subscriptionName}")]
    public async Task<IActionResult> DeleteSubscriptionAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscriptionName,
        CancellationToken cancellation = default)
    {
        await _namespace.DeleteSubscriptionAsync(topicName, subscriptionName, cancellation);
        return NoContent();
    }

    [HttpPost("topics/{topicName}/subscriptions/{subscriptionName}/rules/{ruleName}")]
    public async Task<IActionResult> CreateRuleAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscriptionName,
        [FromRoute] string ruleName,
        [FromBody] RuleSettings settings,
        CancellationToken cancellation = default)
        => Ok(await _namespace.CreateRuleAsync(topicName, subscriptionName, ruleName, settings, cancellation));

    [HttpPut("topics/{topicName}/subscriptions/{subscriptionName}/rules/{ruleName}")]
    public async Task<IActionResult> UpdateRuleAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscriptionName,
        [FromRoute] string ruleName,
        [FromBody] RuleSettings settings,
        CancellationToken cancellation = default)
    {
        await _namespace.UpdateRuleAsync(topicName, subscriptionName, ruleName, settings, cancellation);
        return NoContent();
    }

    [HttpDelete("topics/{topicName}/subscriptions/{subscriptionName}/rules/{ruleName}")]
    public async Task<IActionResult> DeleteRuleAsync(
        [FromRoute] string topicName,
        [FromRoute] string subscriptionName,
        [FromRoute] string ruleName,
        CancellationToken cancellation = default)
    {
        await _namespace.DeleteRuleAsync(topicName, subscriptionName, ruleName, cancellation);
        return NoContent();
    }

    [HttpPost("queues/{queueName}")]
    public async Task<IActionResult> CreateQueueAsync(
        [FromRoute] string queueName,
        [FromBody] QueueSettings settings,
        CancellationToken cancellation = default)
        => Ok(await _namespace.CreateQueueAsync(queueName, settings, cancellation));

    [HttpPut("queues/{queueName}")]
    public async Task<IActionResult> UpdateQueueAsync(
        [FromRoute] string queueName,
        [FromBody] QueueSettings settings,
        CancellationToken cancellation = default)
    {
        await _namespace.UpdateQueueAsync(queueName, settings, cancellation);
        return NoContent();
    }

    [HttpDelete("queues/{queueName}")]
    public async Task<IActionResult> DeleteQueueAsync(
        [FromRoute] string queueName,
        CancellationToken cancellation = default)
    {
        await _namespace.DeleteQueueAsync(queueName, cancellation);
        return NoContent();
    }
}
