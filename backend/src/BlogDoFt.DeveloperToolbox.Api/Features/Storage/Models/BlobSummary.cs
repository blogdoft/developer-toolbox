namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;

public record BlobSummary(string Name, long? Size, string? ContentType, DateTimeOffset? LastModified);
