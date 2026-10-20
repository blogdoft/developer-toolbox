using Azure.Storage.Queues;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage.Impl;

public class QueueStorageService : IQueueStorageService
{
    private const int MaxPeekMessages = 32;

    private readonly QueueServiceClient _client;

    public QueueStorageService(QueueServiceClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<StorageQueueSummary>> GetQueuesAsync(CancellationToken cancellation = default)
    {
        var queues = new List<StorageQueueSummary>();
        await foreach (var queue in _client.GetQueuesAsync(cancellationToken: cancellation))
        {
            queues.Add(await ToSummaryAsync(queue.Name, cancellation));
        }

        return queues;
    }

    public async Task CreateQueueAsync(string queue, CancellationToken cancellation = default)
        => await _client.CreateQueueAsync(queue, cancellationToken: cancellation);

    public async Task DeleteQueueAsync(string queue, CancellationToken cancellation = default)
        => await _client.DeleteQueueAsync(queue, cancellation);

    public async Task<IReadOnlyList<StorageQueueMessage>> PeekMessagesAsync(
        string queue,
        int maxMessages,
        CancellationToken cancellation = default)
    {
        var messages = await _client.GetQueueClient(queue)
            .PeekMessagesAsync(Math.Clamp(maxMessages, 1, MaxPeekMessages), cancellation);

        return messages.Value
            .Select(m => new StorageQueueMessage(
                m.MessageId,
                m.Body.ToString(),
                m.InsertedOn,
                m.ExpiresOn,
                m.DequeueCount))
            .ToList();
    }

    public async Task SendMessageAsync(string queue, string text, CancellationToken cancellation = default)
        => await _client.GetQueueClient(queue).SendMessageAsync(text, cancellation);

    public async Task ClearMessagesAsync(string queue, CancellationToken cancellation = default)
        => await _client.GetQueueClient(queue).ClearMessagesAsync(cancellation);

    private async Task<StorageQueueSummary> ToSummaryAsync(string queue, CancellationToken cancellation)
    {
        var properties = await _client.GetQueueClient(queue).GetPropertiesAsync(cancellation);
        return new(queue, properties.Value.ApproximateMessagesCount);
    }
}
