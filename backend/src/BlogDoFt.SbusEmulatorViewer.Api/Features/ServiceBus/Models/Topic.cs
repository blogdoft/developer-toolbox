namespace BlogDoFt.SbusEmulatorViewer.Api.Features.ServiceBus.Models;

public record Topic(string TopicName, IReadOnlyList<string> Subscriptions);
