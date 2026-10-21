namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;

public record BlobContainerSummary(string Name, DateTimeOffset? LastModified, string? PublicAccess);
