using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage.Impl;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _client;

    public BlobStorageService(BlobServiceClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<BlobContainerSummary>> GetContainersAsync(
        CancellationToken cancellation = default)
    {
        var containers = new List<BlobContainerSummary>();
        await foreach (var container in _client.GetBlobContainersAsync(cancellationToken: cancellation))
        {
            containers.Add(new(
                container.Name,
                container.Properties.LastModified,
                container.Properties.PublicAccess?.ToString()));
        }

        return containers;
    }

    public async Task CreateContainerAsync(string container, CancellationToken cancellation = default)
        => await _client.CreateBlobContainerAsync(container, cancellationToken: cancellation);

    public async Task DeleteContainerAsync(string container, CancellationToken cancellation = default)
        => await _client.DeleteBlobContainerAsync(container, cancellationToken: cancellation);

    public async Task<IReadOnlyList<BlobSummary>> GetBlobsAsync(
        string container,
        string? prefix,
        CancellationToken cancellation = default)
    {
        var blobs = new List<BlobSummary>();
        var containerClient = _client.GetBlobContainerClient(container);
        await foreach (var blob in containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, cancellation))
        {
            blobs.Add(new(
                blob.Name,
                blob.Properties.ContentLength,
                blob.Properties.ContentType,
                blob.Properties.LastModified));
        }

        return blobs;
    }

    public async Task UploadBlobAsync(
        string container,
        string blobName,
        Stream content,
        string? contentType,
        CancellationToken cancellation = default)
    {
        var blob = _client.GetBlobContainerClient(container).GetBlobClient(blobName);
        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType ?? "application/octet-stream" },
        };

        await blob.UploadAsync(content, options, cancellation);
    }

    public async Task<BlobContent> DownloadBlobAsync(
        string container,
        string blobName,
        CancellationToken cancellation = default)
    {
        var blob = _client.GetBlobContainerClient(container).GetBlobClient(blobName);
        var download = await blob.DownloadStreamingAsync(cancellationToken: cancellation);

        return new(download.Value.Content, download.Value.Details.ContentType);
    }

    public async Task DeleteBlobAsync(string container, string blobName, CancellationToken cancellation = default)
        => await _client.GetBlobContainerClient(container).DeleteBlobAsync(blobName, cancellationToken: cancellation);
}
