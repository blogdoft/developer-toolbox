namespace BlogDoFt.SbusEmulatorViewer.Api.Services;

public interface IServiceBusService
{
    Task SendMessageAsync(string entityName, object message);

    Task<IEnumerable<object>> TopicReceiveMessagesAsync(
        string entityName,
        string subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default);
}
