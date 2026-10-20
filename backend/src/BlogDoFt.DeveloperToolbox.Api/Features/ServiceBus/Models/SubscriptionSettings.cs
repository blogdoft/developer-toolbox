namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// Subscription settings. Durations are ISO 8601 strings. On update, <see cref="RequiresSession"/>
/// is ignored because it is fixed at creation.
/// </summary>
public class SubscriptionSettings
{
    [Iso8601Duration]
    public string? LockDuration { get; init; }

    [Iso8601Duration]
    public string? DefaultMessageTimeToLive { get; init; }

    [Iso8601Duration]
    public string? AutoDeleteOnIdle { get; init; }

    public int? MaxDeliveryCount { get; init; }

    public bool? DeadLetteringOnMessageExpiration { get; init; }

    public bool? RequiresSession { get; init; }

    public string? ForwardTo { get; init; }

    public string? ForwardDeadLetteredMessagesTo { get; init; }
}
