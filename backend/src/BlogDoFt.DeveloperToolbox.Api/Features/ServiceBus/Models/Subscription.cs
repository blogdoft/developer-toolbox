namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// A topic subscription, its settings and filter rules.
/// </summary>
public record Subscription(
    string Name,
    SubscriptionSettings Properties,
    IReadOnlyList<RuleInfo> Rules);
