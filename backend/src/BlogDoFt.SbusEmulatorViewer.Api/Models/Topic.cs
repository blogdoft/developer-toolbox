namespace BlogDoFt.SbusEmulatorViewer.Api.Models;

public record Topic(string TopicName, IReadOnlyList<string> Subscriptions);
