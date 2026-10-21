namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;

public record StorageQueueMessage(
    string MessageId,
    string Body,
    DateTimeOffset? InsertedOn,
    DateTimeOffset? ExpiresOn,
    long DequeueCount);
