using Azure.Messaging.ServiceBus.Administration;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Impl;

/// <summary>
/// Reads and manages the entities of a real Service Bus namespace through the administration client.
/// </summary>
public class AdministrationClientNamespaceManager : INamespaceManager
{
    private readonly ServiceBusAdministrationClient _admin;

    public AdministrationClientNamespaceManager(ServiceBusAdministrationClient admin)
    {
        _admin = admin;
    }

    public async Task<IReadOnlyList<Topic>> GetTopicsAsync(CancellationToken cancellation = default)
    {
        var topics = new List<Topic>();
        await foreach (var topic in _admin.GetTopicsAsync(cancellation))
        {
            var subscriptions = new List<Subscription>();
            await foreach (var sub in _admin.GetSubscriptionsAsync(topic.Name, cancellation))
            {
                subscriptions.Add(await ToSubscriptionAsync(sub, cancellation));
            }

            topics.Add(new Topic(topic.Name, ToSettings(topic), subscriptions));
        }

        return topics;
    }

    public async Task<IReadOnlyList<Queue>> GetQueuesAsync(CancellationToken cancellation = default)
    {
        var queues = new List<Queue>();
        await foreach (var queue in _admin.GetQueuesAsync(cancellation))
        {
            queues.Add(new Queue(queue.Name, ToSettings(queue)));
        }

        return queues;
    }

    public async Task<Topic> CreateTopicAsync(
        string name,
        TopicSettings settings,
        CancellationToken cancellation = default)
    {
        var options = new CreateTopicOptions(name);
        ApplyTo(settings, options);
        var created = await _admin.CreateTopicAsync(options, cancellation);
        return new Topic(created.Value.Name, ToSettings(created.Value), []);
    }

    public async Task UpdateTopicAsync(string name, TopicSettings settings, CancellationToken cancellation = default)
    {
        var topic = (await _admin.GetTopicAsync(name, cancellation)).Value;
        topic.DefaultMessageTimeToLive = Iso8601DurationAttribute.Parse(settings.DefaultMessageTimeToLive)
            ?? topic.DefaultMessageTimeToLive;
        topic.DuplicateDetectionHistoryTimeWindow = Iso8601DurationAttribute.Parse(settings.DuplicateDetectionHistoryTimeWindow)
            ?? topic.DuplicateDetectionHistoryTimeWindow;
        topic.AutoDeleteOnIdle = Iso8601DurationAttribute.Parse(settings.AutoDeleteOnIdle)
            ?? topic.AutoDeleteOnIdle;
        await _admin.UpdateTopicAsync(topic, cancellation);
    }

    public async Task DeleteTopicAsync(string name, CancellationToken cancellation = default)
        => await _admin.DeleteTopicAsync(name, cancellation);

    public async Task<Subscription> CreateSubscriptionAsync(
        string topicName,
        string name,
        SubscriptionSettings settings,
        CancellationToken cancellation = default)
    {
        var options = new CreateSubscriptionOptions(topicName, name);
        ApplyTo(settings, options);
        var created = await _admin.CreateSubscriptionAsync(options, cancellation);
        return await ToSubscriptionAsync(created.Value, cancellation);
    }

    public async Task UpdateSubscriptionAsync(
        string topicName,
        string name,
        SubscriptionSettings settings,
        CancellationToken cancellation = default)
    {
        var sub = (await _admin.GetSubscriptionAsync(topicName, name, cancellation)).Value;
        sub.LockDuration = Iso8601DurationAttribute.Parse(settings.LockDuration) ?? sub.LockDuration;
        sub.DefaultMessageTimeToLive = Iso8601DurationAttribute.Parse(settings.DefaultMessageTimeToLive)
            ?? sub.DefaultMessageTimeToLive;
        sub.AutoDeleteOnIdle = Iso8601DurationAttribute.Parse(settings.AutoDeleteOnIdle) ?? sub.AutoDeleteOnIdle;
        sub.MaxDeliveryCount = settings.MaxDeliveryCount ?? sub.MaxDeliveryCount;
        sub.DeadLetteringOnMessageExpiration = settings.DeadLetteringOnMessageExpiration
            ?? sub.DeadLetteringOnMessageExpiration;
        sub.ForwardTo = settings.ForwardTo;
        sub.ForwardDeadLetteredMessagesTo = settings.ForwardDeadLetteredMessagesTo;
        await _admin.UpdateSubscriptionAsync(sub, cancellation);
    }

    public async Task DeleteSubscriptionAsync(
        string topicName,
        string name,
        CancellationToken cancellation = default)
        => await _admin.DeleteSubscriptionAsync(topicName, name, cancellation);

    public async Task<RuleInfo> CreateRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        RuleSettings settings,
        CancellationToken cancellation = default)
    {
        var options = new CreateRuleOptions(name, ToFilter(settings))
        {
            Action = ToAction(settings),
        };
        var created = await _admin.CreateRuleAsync(topicName, subscriptionName, options, cancellation);
        return ToRule(created.Value);
    }

    public async Task UpdateRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        RuleSettings settings,
        CancellationToken cancellation = default)
    {
        var rule = (await _admin.GetRuleAsync(topicName, subscriptionName, name, cancellation)).Value;
        rule.Filter = ToFilter(settings);
        rule.Action = ToAction(settings);
        await _admin.UpdateRuleAsync(topicName, subscriptionName, rule, cancellation);
    }

    public async Task DeleteRuleAsync(
        string topicName,
        string subscriptionName,
        string name,
        CancellationToken cancellation = default)
        => await _admin.DeleteRuleAsync(topicName, subscriptionName, name, cancellation);

    public async Task<Queue> CreateQueueAsync(
        string name,
        QueueSettings settings,
        CancellationToken cancellation = default)
    {
        var options = new CreateQueueOptions(name);
        ApplyTo(settings, options);
        var created = await _admin.CreateQueueAsync(options, cancellation);
        return new Queue(created.Value.Name, ToSettings(created.Value));
    }

    public async Task UpdateQueueAsync(string name, QueueSettings settings, CancellationToken cancellation = default)
    {
        var queue = (await _admin.GetQueueAsync(name, cancellation)).Value;
        queue.LockDuration = Iso8601DurationAttribute.Parse(settings.LockDuration) ?? queue.LockDuration;
        queue.DefaultMessageTimeToLive = Iso8601DurationAttribute.Parse(settings.DefaultMessageTimeToLive)
            ?? queue.DefaultMessageTimeToLive;
        queue.DuplicateDetectionHistoryTimeWindow = Iso8601DurationAttribute.Parse(settings.DuplicateDetectionHistoryTimeWindow)
            ?? queue.DuplicateDetectionHistoryTimeWindow;
        queue.AutoDeleteOnIdle = Iso8601DurationAttribute.Parse(settings.AutoDeleteOnIdle) ?? queue.AutoDeleteOnIdle;
        queue.MaxDeliveryCount = settings.MaxDeliveryCount ?? queue.MaxDeliveryCount;
        queue.DeadLetteringOnMessageExpiration = settings.DeadLetteringOnMessageExpiration
            ?? queue.DeadLetteringOnMessageExpiration;
        queue.ForwardTo = settings.ForwardTo;
        queue.ForwardDeadLetteredMessagesTo = settings.ForwardDeadLetteredMessagesTo;
        await _admin.UpdateQueueAsync(queue, cancellation);
    }

    public async Task DeleteQueueAsync(string name, CancellationToken cancellation = default)
        => await _admin.DeleteQueueAsync(name, cancellation);

    private static void ApplyTo(TopicSettings settings, CreateTopicOptions options)
    {
        options.DefaultMessageTimeToLive = Iso8601DurationAttribute.Parse(settings.DefaultMessageTimeToLive)
            ?? options.DefaultMessageTimeToLive;
        options.DuplicateDetectionHistoryTimeWindow = Iso8601DurationAttribute.Parse(settings.DuplicateDetectionHistoryTimeWindow)
            ?? options.DuplicateDetectionHistoryTimeWindow;
        options.AutoDeleteOnIdle = Iso8601DurationAttribute.Parse(settings.AutoDeleteOnIdle)
            ?? options.AutoDeleteOnIdle;
        options.RequiresDuplicateDetection = settings.RequiresDuplicateDetection ?? options.RequiresDuplicateDetection;
        options.MaxSizeInMegabytes = settings.MaxSizeInMegabytes ?? options.MaxSizeInMegabytes;
    }

    private static void ApplyTo(SubscriptionSettings settings, CreateSubscriptionOptions options)
    {
        options.LockDuration = Iso8601DurationAttribute.Parse(settings.LockDuration) ?? options.LockDuration;
        options.DefaultMessageTimeToLive = Iso8601DurationAttribute.Parse(settings.DefaultMessageTimeToLive)
            ?? options.DefaultMessageTimeToLive;
        options.AutoDeleteOnIdle = Iso8601DurationAttribute.Parse(settings.AutoDeleteOnIdle)
            ?? options.AutoDeleteOnIdle;
        options.MaxDeliveryCount = settings.MaxDeliveryCount ?? options.MaxDeliveryCount;
        options.DeadLetteringOnMessageExpiration = settings.DeadLetteringOnMessageExpiration
            ?? options.DeadLetteringOnMessageExpiration;
        options.RequiresSession = settings.RequiresSession ?? options.RequiresSession;
        options.ForwardTo = NullIfEmpty(settings.ForwardTo);
        options.ForwardDeadLetteredMessagesTo = NullIfEmpty(settings.ForwardDeadLetteredMessagesTo);
    }

    private static void ApplyTo(QueueSettings settings, CreateQueueOptions options)
    {
        options.LockDuration = Iso8601DurationAttribute.Parse(settings.LockDuration) ?? options.LockDuration;
        options.DefaultMessageTimeToLive = Iso8601DurationAttribute.Parse(settings.DefaultMessageTimeToLive)
            ?? options.DefaultMessageTimeToLive;
        options.DuplicateDetectionHistoryTimeWindow = Iso8601DurationAttribute.Parse(settings.DuplicateDetectionHistoryTimeWindow)
            ?? options.DuplicateDetectionHistoryTimeWindow;
        options.AutoDeleteOnIdle = Iso8601DurationAttribute.Parse(settings.AutoDeleteOnIdle)
            ?? options.AutoDeleteOnIdle;
        options.MaxDeliveryCount = settings.MaxDeliveryCount ?? options.MaxDeliveryCount;
        options.DeadLetteringOnMessageExpiration = settings.DeadLetteringOnMessageExpiration
            ?? options.DeadLetteringOnMessageExpiration;
        options.RequiresSession = settings.RequiresSession ?? options.RequiresSession;
        options.RequiresDuplicateDetection = settings.RequiresDuplicateDetection ?? options.RequiresDuplicateDetection;
        options.MaxSizeInMegabytes = settings.MaxSizeInMegabytes ?? options.MaxSizeInMegabytes;
        options.ForwardTo = NullIfEmpty(settings.ForwardTo);
        options.ForwardDeadLetteredMessagesTo = NullIfEmpty(settings.ForwardDeadLetteredMessagesTo);
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private static TopicSettings ToSettings(TopicProperties topic) => new()
    {
        DefaultMessageTimeToLive = Iso8601DurationAttribute.Format(topic.DefaultMessageTimeToLive),
        DuplicateDetectionHistoryTimeWindow = Iso8601DurationAttribute.Format(topic.DuplicateDetectionHistoryTimeWindow),
        AutoDeleteOnIdle = Iso8601DurationAttribute.Format(topic.AutoDeleteOnIdle),
        RequiresDuplicateDetection = topic.RequiresDuplicateDetection,
        MaxSizeInMegabytes = topic.MaxSizeInMegabytes,
    };

    private static SubscriptionSettings ToSettings(SubscriptionProperties sub) => new()
    {
        LockDuration = Iso8601DurationAttribute.Format(sub.LockDuration),
        DefaultMessageTimeToLive = Iso8601DurationAttribute.Format(sub.DefaultMessageTimeToLive),
        AutoDeleteOnIdle = Iso8601DurationAttribute.Format(sub.AutoDeleteOnIdle),
        MaxDeliveryCount = sub.MaxDeliveryCount,
        DeadLetteringOnMessageExpiration = sub.DeadLetteringOnMessageExpiration,
        RequiresSession = sub.RequiresSession,
        ForwardTo = NullIfEmpty(sub.ForwardTo),
        ForwardDeadLetteredMessagesTo = NullIfEmpty(sub.ForwardDeadLetteredMessagesTo),
    };

    private static QueueSettings ToSettings(QueueProperties queue) => new()
    {
        LockDuration = Iso8601DurationAttribute.Format(queue.LockDuration),
        DefaultMessageTimeToLive = Iso8601DurationAttribute.Format(queue.DefaultMessageTimeToLive),
        DuplicateDetectionHistoryTimeWindow = Iso8601DurationAttribute.Format(queue.DuplicateDetectionHistoryTimeWindow),
        AutoDeleteOnIdle = Iso8601DurationAttribute.Format(queue.AutoDeleteOnIdle),
        MaxDeliveryCount = queue.MaxDeliveryCount,
        DeadLetteringOnMessageExpiration = queue.DeadLetteringOnMessageExpiration,
        RequiresSession = queue.RequiresSession,
        RequiresDuplicateDetection = queue.RequiresDuplicateDetection,
        MaxSizeInMegabytes = queue.MaxSizeInMegabytes,
        ForwardTo = NullIfEmpty(queue.ForwardTo),
        ForwardDeadLetteredMessagesTo = NullIfEmpty(queue.ForwardDeadLetteredMessagesTo),
    };

    private static RuleFilter ToFilter(RuleSettings settings)
    {
        if (settings.FilterType!.Equals("Sql", StringComparison.OrdinalIgnoreCase))
        {
            return new SqlRuleFilter(settings.SqlExpression);
        }

        if (settings.FilterType.Equals("Correlation", StringComparison.OrdinalIgnoreCase))
        {
            var filter = new CorrelationRuleFilter
            {
                CorrelationId = NullIfEmpty(settings.CorrelationId),
                MessageId = NullIfEmpty(settings.MessageId),
                To = NullIfEmpty(settings.To),
                ReplyTo = NullIfEmpty(settings.ReplyTo),
                Subject = NullIfEmpty(settings.Subject),
                SessionId = NullIfEmpty(settings.SessionId),
                ReplyToSessionId = NullIfEmpty(settings.ReplyToSessionId),
                ContentType = NullIfEmpty(settings.ContentType),
            };
            foreach (var (key, value) in settings.ApplicationProperties ?? [])
            {
                filter.ApplicationProperties[key] = value;
            }

            return filter;
        }

        return settings.FilterType.Equals("True", StringComparison.OrdinalIgnoreCase)
            ? new TrueRuleFilter()
            : new FalseRuleFilter();
    }

    private static RuleAction? ToAction(RuleSettings settings)
        => string.IsNullOrWhiteSpace(settings.ActionExpression)
            ? null
            : new SqlRuleAction(settings.ActionExpression);

    private static RuleInfo ToRule(RuleProperties rule)
    {
        var action = (rule.Action as SqlRuleAction)?.SqlExpression;
        var settings = rule.Filter switch
        {
            TrueRuleFilter => new RuleSettings { FilterType = "True", ActionExpression = action },
            FalseRuleFilter => new RuleSettings { FilterType = "False", ActionExpression = action },
            SqlRuleFilter sql => new RuleSettings
            {
                FilterType = "Sql",
                SqlExpression = sql.SqlExpression,
                ActionExpression = action,
            },
            CorrelationRuleFilter c => new RuleSettings
            {
                FilterType = "Correlation",
                ActionExpression = action,
                CorrelationId = c.CorrelationId,
                MessageId = c.MessageId,
                To = c.To,
                ReplyTo = c.ReplyTo,
                Subject = c.Subject,
                SessionId = c.SessionId,
                ReplyToSessionId = c.ReplyToSessionId,
                ContentType = c.ContentType,
                ApplicationProperties = c.ApplicationProperties
                    .ToDictionary(p => p.Key, p => p.Value?.ToString() ?? string.Empty),
            },
            _ => new RuleSettings { ActionExpression = action },
        };

        return new RuleInfo(rule.Name, settings);
    }

    private async Task<Subscription> ToSubscriptionAsync(
        SubscriptionProperties sub,
        CancellationToken cancellation)
    {
        var rules = new List<RuleInfo>();
        await foreach (var rule in _admin.GetRulesAsync(sub.TopicName, sub.SubscriptionName, cancellation))
        {
            rules.Add(ToRule(rule));
        }

        return new Subscription(sub.SubscriptionName, ToSettings(sub), rules);
    }
}
