using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage;

/// <summary>
/// Manages blob containers and blobs of the connected storage account.
/// </summary>
public interface IBlobStorageService
{
    Task<IReadOnlyList<BlobContainerSummary>> GetContainersAsync(CancellationToken cancellation = default);

    Task CreateContainerAsync(string container, CancellationToken cancellation = default);

    Task DeleteContainerAsync(string container, CancellationToken cancellation = default);

    Task<IReadOnlyList<BlobSummary>> GetBlobsAsync(
        string container,
        string? prefix,
        CancellationToken cancellation = default);

    Task UploadBlobAsync(
        string container,
        string blobName,
        Stream content,
        string? contentType,
        CancellationToken cancellation = default);

    Task<BlobContent> DownloadBlobAsync(string container, string blobName, CancellationToken cancellation = default);

    Task DeleteBlobAsync(string container, string blobName, CancellationToken cancellation = default);
}
