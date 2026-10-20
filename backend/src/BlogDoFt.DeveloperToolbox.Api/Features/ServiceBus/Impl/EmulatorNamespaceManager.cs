using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using BlogDoFt.DeveloperToolbox.Api.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Impl;

/// <summary>
/// Reads the entities from the emulator config file. The emulator has no management API,
/// so every management operation is rejected.
/// </summary>
public class EmulatorNamespaceManager : INamespaceManager
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly string? _configPath;
    private readonly Lazy<Task<Snapshot>> _snapshot;

    public EmulatorNamespaceManager(IOptions<ServiceBusSettings> sbusSettings)
    {
        _configPath = sbusSettings.Value.EmulatorConfigFile;
        _snapshot = new Lazy<Task<Snapshot>>(LoadAsync);
    }

    public async Task<IReadOnlyList<Topic>> GetTopicsAsync(CancellationToken cancellation = default)
        => (await _snapshot.Value).Topics;

    public async Task<IReadOnlyList<Queue>> GetQueuesAsync(CancellationToken cancellation = default)
        => (await _snapshot.Value).Queues;

    public Task<Topic> CreateTopicAsync(string name, TopicSettings settings, CancellationToken cancellation = default)
        => throw Unsupported("create topic");

    public Task UpdateTopicAsync(string name, TopicSettings settings, CancellationToken cancellation = default)
        => throw Unsupported("update topic");

    public Task DeleteTopicAsync(string name, CancellationToken cancellation = default)
        => throw Unsupported("delete topic");

    public Task<Subscription> CreateSubscriptionAsync(
        string topicName,
        string name,
        SubscriptionSettings settings,
        CancellationToken cancellation = default)
        => throw Unsupported("create subscription");

    public Task UpdateSubscriptionAsync(
        string topicName,
        string name,
        SubscriptionSettings settings,
        CancellationToken cancellation = default)
        => throw Unsupported("update subscription");

    public Task DeleteSubscriptionAsync(string topicName, string name, CancellationToken cancellation = default)
        => throw Unsupported("delete subscription");

    public Task<RuleInfo> CreateRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        RuleSettings settings,
        CancellationToken cancellation = default)
        => throw Unsupported("create rule");

    public Task UpdateRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        RuleSettings settings,
        CancellationToken cancellation = default)
        => throw Unsupported("update rule");

    public Task DeleteRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        CancellationToken cancellation = default)
        => throw Unsupported("delete rule");

    public Task<Queue> CreateQueueAsync(string name, QueueSettings settings, CancellationToken cancellation = default)
        => throw Unsupported("create queue");

    public Task UpdateQueueAsync(string name, QueueSettings settings, CancellationToken cancellation = default)
        => throw Unsupported("update queue");

    public Task DeleteQueueAsync(string name, CancellationToken cancellation = default)
        => throw Unsupported("delete queue");

    private static NamespaceOperationNotSupportedException Unsupported(string operation) => new(operation);

    private static RuleInfo ToRule(RuleConfig rule)
    {
        var props = rule.Properties;
        var correlation = props?.CorrelationFilter;
        return new RuleInfo(
            rule.Name!,
            new RuleSettings
            {
                FilterType = props?.FilterType,
                SqlExpression = props?.SqlFilter?.SqlExpression,
                ActionExpression = props?.Action?.SqlExpression,
                CorrelationId = correlation?.CorrelationId,
                MessageId = correlation?.MessageId,
                To = correlation?.To,
                ReplyTo = correlation?.ReplyTo,
                Subject = correlation?.Label,
                SessionId = correlation?.SessionId,
                ReplyToSessionId = correlation?.ReplyToSessionId,
                ContentType = correlation?.ContentType,
                ApplicationProperties = correlation?.Properties,
            });
    }

    private async Task<Snapshot> LoadAsync()
    {
        if (string.IsNullOrWhiteSpace(_configPath) || !File.Exists(_configPath))
        {
            throw new FileNotFoundException(
                "Arquivo de configuração não encontrado.", _configPath);
        }

        await using var stream = File.OpenRead(_configPath);
        var root = await JsonSerializer.DeserializeAsync<RootConfig>(stream, JsonOptions);
        var namespaces = root?.UserConfig?.Namespaces ?? [];

        var topics = namespaces
            .SelectMany(ns => ns.Topics ?? [])
            .Select(t => new Topic(
                t.Name!,
                t.Properties ?? new TopicSettings(),
                (t.Subscriptions ?? [])
                    .Select(s => new Subscription(
                        s.Name!,
                        s.Properties ?? new SubscriptionSettings(),
                        (s.Rules ?? []).Select(ToRule).ToArray()))
                    .ToArray()))
            .ToArray();

        var queues = namespaces
            .SelectMany(ns => ns.Queues ?? [])
            .Select(q => new Queue(q.Name!, q.Properties ?? new QueueSettings()))
            .ToArray();

        return new Snapshot(topics, queues);
    }

    private sealed record Snapshot(IReadOnlyList<Topic> Topics, IReadOnlyList<Queue> Queues);

    private sealed record SqlFilterConfig(string? SqlExpression);

    private sealed record CorrelationFilterConfig(
        string? CorrelationId,
        string? MessageId,
        string? To,
        string? ReplyTo,
        string? Label,
        string? SessionId,
        string? ReplyToSessionId,
        string? ContentType,
        Dictionary<string, string>? Properties);

    private sealed record RuleProperties(
        string? FilterType,
        SqlFilterConfig? SqlFilter,
        CorrelationFilterConfig? CorrelationFilter,
        SqlFilterConfig? Action);

    private sealed record RuleConfig(string? Name, RuleProperties? Properties);

    private sealed record SubscriptionConfig(string? Name, SubscriptionSettings? Properties, RuleConfig[]? Rules);

    private sealed record TopicConfig(string? Name, TopicSettings? Properties, SubscriptionConfig[]? Subscriptions);

    private sealed record QueueConfig(string? Name, QueueSettings? Properties);

    private sealed record NamespaceConfig(TopicConfig[]? Topics, QueueConfig[]? Queues);

    private sealed record UserConfig(NamespaceConfig[]? Namespaces);

    private sealed record RootConfig(UserConfig? UserConfig);
}
