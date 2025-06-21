namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

public record Topic(string TopicName, IReadOnlyList<string> Subscriptions);
