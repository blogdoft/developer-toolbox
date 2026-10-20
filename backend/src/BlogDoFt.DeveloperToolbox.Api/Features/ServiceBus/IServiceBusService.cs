namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

/// <summary>
/// Sends and reads messages. When <c>subscription</c> is null the entity is a queue,
/// otherwise it is a topic.
/// </summary>
public interface IServiceBusService
{
    Task SendMessageAsync(string entityName, object message);

    /// <summary>
    /// Reads messages without removing them from the entity.
    /// </summary>
    Task<IEnumerable<object>> PeekMessagesAsync(
        string entityName,
        string? subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default);

    /// <summary>
    /// Reads messages and removes them from the entity.
    /// </summary>
    Task<IEnumerable<object>> ReceiveMessagesAsync(
        string entityName,
        string? subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default);

    /// <summary>
    /// Removes every active message from the entity.
    /// </summary>
    /// <returns>The number of removed messages.</returns>
    Task<int> PurgeMessagesAsync(
        string entityName,
        string? subscription,
        CancellationToken cancellation = default);
}
