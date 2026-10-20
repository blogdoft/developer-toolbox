namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// A topic, its settings and subscriptions.
/// </summary>
public record Topic(
    string TopicName,
    TopicSettings Properties,
    IReadOnlyList<Subscription> Subscriptions);
