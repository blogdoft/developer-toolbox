namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// Topic settings. Durations are ISO 8601 strings. On update, only the time-to-live,
/// auto-delete and duplicate detection window are applied; the other fields are fixed at creation.
/// </summary>
public class TopicSettings
{
    [Iso8601Duration]
    public string? DefaultMessageTimeToLive { get; init; }

    [Iso8601Duration]
    public string? DuplicateDetectionHistoryTimeWindow { get; init; }

    [Iso8601Duration]
    public string? AutoDeleteOnIdle { get; init; }

    public bool? RequiresDuplicateDetection { get; init; }

    public long? MaxSizeInMegabytes { get; init; }
}
