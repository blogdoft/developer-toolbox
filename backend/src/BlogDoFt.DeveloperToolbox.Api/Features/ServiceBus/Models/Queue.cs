namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// A queue and its settings.
/// </summary>
public record Queue(string Name, QueueSettings Properties);
