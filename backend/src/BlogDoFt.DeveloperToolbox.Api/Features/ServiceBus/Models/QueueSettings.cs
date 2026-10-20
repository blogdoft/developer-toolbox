namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// Queue settings. Durations are ISO 8601 strings. On update, <see cref="RequiresSession"/>,
/// <see cref="RequiresDuplicateDetection"/> and <see cref="MaxSizeInMegabytes"/> are ignored
/// because they are fixed at creation.
/// </summary>
public class QueueSettings
{
    [Iso8601Duration]
    public string? LockDuration { get; init; }

    [Iso8601Duration]
    public string? DefaultMessageTimeToLive { get; init; }

    [Iso8601Duration]
    public string? DuplicateDetectionHistoryTimeWindow { get; init; }

    [Iso8601Duration]
    public string? AutoDeleteOnIdle { get; init; }

    public int? MaxDeliveryCount { get; init; }

    public bool? DeadLetteringOnMessageExpiration { get; init; }

    public bool? RequiresSession { get; init; }

    public bool? RequiresDuplicateDetection { get; init; }

    public long? MaxSizeInMegabytes { get; init; }

    public string? ForwardTo { get; init; }

    public string? ForwardDeadLetteredMessagesTo { get; init; }
}
