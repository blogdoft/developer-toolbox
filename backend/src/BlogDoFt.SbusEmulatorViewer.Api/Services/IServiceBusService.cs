namespace BlogDoFt.SbusEmulatorViewer.Api.Services;

public interface IServiceBusService
{
    Task SendMessageAsync(string entityName, string message);

    Task<IEnumerable<string>> TopicReceiveMessagesAsync(
        string entityName,
        string subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default);
}
