using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

/// <summary>
/// Reads and manages the entities of the connected Service Bus namespace.
/// Management operations throw <see cref="NamespaceOperationNotSupportedException"/>
/// when the target cannot perform them (the emulator).
/// </summary>
public interface INamespaceManager
{
    Task<IReadOnlyList<Topic>> GetTopicsAsync(CancellationToken cancellation = default);

    Task<IReadOnlyList<Queue>> GetQueuesAsync(CancellationToken cancellation = default);

    Task<Topic> CreateTopicAsync(string name, TopicSettings settings, CancellationToken cancellation = default);

    Task UpdateTopicAsync(string name, TopicSettings settings, CancellationToken cancellation = default);

    Task DeleteTopicAsync(string name, CancellationToken cancellation = default);

    Task<Subscription> CreateSubscriptionAsync(
        string topicName,
        string name,
        SubscriptionSettings settings,
        CancellationToken cancellation = default);

    Task UpdateSubscriptionAsync(
        string topicName,
        string name,
        SubscriptionSettings settings,
        CancellationToken cancellation = default);

    Task DeleteSubscriptionAsync(string topicName, string name, CancellationToken cancellation = default);

    Task<RuleInfo> CreateRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        RuleSettings settings,
        CancellationToken cancellation = default);

    Task UpdateRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        RuleSettings settings,
        CancellationToken cancellation = default);

    Task DeleteRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        CancellationToken cancellation = default);

    Task<Queue> CreateQueueAsync(string name, QueueSettings settings, CancellationToken cancellation = default);

    Task UpdateQueueAsync(string name, QueueSettings settings, CancellationToken cancellation = default);

    Task DeleteQueueAsync(string name, CancellationToken cancellation = default);
}
