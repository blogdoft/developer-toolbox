namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

public interface IServiceBusService
{
    Task SendMessageAsync(string entityName, object message);

    Task<IEnumerable<object>> TopicReceiveMessagesAsync(
        string entityName,
        string subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default);
}
