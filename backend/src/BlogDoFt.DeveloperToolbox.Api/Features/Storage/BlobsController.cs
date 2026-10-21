using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage;

[ApiController]
[Route("api/storage/blobs")]
[Tags("storage-blobs")]
public class BlobsController : ControllerBase
{
    private readonly IBlobStorageService _service;

    public BlobsController(IBlobStorageService service)
    {
        _service = service;
    }

    [HttpGet("containers")]
    public async Task<IActionResult> GetContainersAsync(CancellationToken cancellation)
        => Ok(await _service.GetContainersAsync(cancellation));

    [HttpPost("containers/{container}")]
    public async Task<IActionResult> CreateContainerAsync(string container, CancellationToken cancellation)
    {
        await _service.CreateContainerAsync(container, cancellation);
        return Created();
    }

    [HttpDelete("containers/{container}")]
    public async Task<IActionResult> DeleteContainerAsync(string container, CancellationToken cancellation)
    {
        await _service.DeleteContainerAsync(container, cancellation);
        return NoContent();
    }

    [HttpGet("containers/{container}/blobs")]
    public async Task<IActionResult> GetBlobsAsync(
        string container,
        [FromQuery] string? prefix,
        CancellationToken cancellation)
        => Ok(await _service.GetBlobsAsync(container, prefix, cancellation));

    /// <summary>
    /// Uploads a file. The blob is named after <paramref name="name"/>, or after the file when omitted.
    /// </summary>
    [HttpPost("containers/{container}/blobs")]
    public async Task<IActionResult> UploadBlobAsync(
        string container,
        IFormFile file,
        [FromForm] string? name,
        CancellationToken cancellation)
    {
        await using var content = file.OpenReadStream();
        await _service.UploadBlobAsync(
            container,
            string.IsNullOrWhiteSpace(name) ? file.FileName : name,
            content,
            file.ContentType,
            cancellation);

        return Created();
    }

    /// <summary>
    /// Blob names can contain slashes, so they travel in the query string.
    /// </summary>
    [HttpGet("containers/{container}/blobs/download")]
    public async Task<IActionResult> DownloadBlobAsync(
        string container,
        [FromQuery] string name,
        CancellationToken cancellation)
    {
        var blob = await _service.DownloadBlobAsync(container, name, cancellation);
        return File(blob.Content, blob.ContentType, Path.GetFileName(name));
    }

    [HttpDelete("containers/{container}/blobs")]
    public async Task<IActionResult> DeleteBlobAsync(
        string container,
        [FromQuery] string name,
        CancellationToken cancellation)
    {
        await _service.DeleteBlobAsync(container, name, cancellation);
        return NoContent();
    }
}
