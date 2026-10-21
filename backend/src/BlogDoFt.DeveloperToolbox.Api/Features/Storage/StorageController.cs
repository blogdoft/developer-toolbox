using Azure.Storage.Blobs;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using BlogDoFt.DeveloperToolbox.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage;

[ApiController]
[Route("api/storage")]
[Tags("storage")]
public class StorageController : ControllerBase
{
    /// <summary>
    /// Tells whether the backend is connected to Azurite or to a real storage account.
    /// </summary>
    [HttpGet]
    public IActionResult GetInfo(
        [FromServices] IOptions<StorageSettings> settings,
        [FromServices] BlobServiceClient client)
        => Ok(new StorageInfo(settings.Value.IsEmulator, client.AccountName));
}
