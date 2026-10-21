using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage;

/// <summary>
/// Manages storage queues and their messages.
/// </summary>
public interface IQueueStorageService
{
    Task<IReadOnlyList<StorageQueueSummary>> GetQueuesAsync(CancellationToken cancellation = default);

    Task CreateQueueAsync(string queue, CancellationToken cancellation = default);

    Task DeleteQueueAsync(string queue, CancellationToken cancellation = default);

    /// <summary>
    /// Reads messages without changing their visibility.
    /// </summary>
    Task<IReadOnlyList<StorageQueueMessage>> PeekMessagesAsync(
        string queue,
        int maxMessages,
        CancellationToken cancellation = default);

    Task SendMessageAsync(string queue, string text, CancellationToken cancellation = default);

    Task ClearMessagesAsync(string queue, CancellationToken cancellation = default);
}
