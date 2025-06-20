using BlogDoFt.SbusEmulatorViewer.Api.Extensions;
using BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Impl;
using BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class SecretsController : ControllerBase
{
    private const string NonSupportedMidia = "{0} is not supported";
    private readonly ISecretCriptographyService _criptographyService;
    private readonly ISecretRepository _repository;

    public SecretsController(
        ISecretCriptographyService criptographyService,
        ISecretRepository repository)
    {
        _criptographyService = criptographyService;
        _repository = repository;
    }

    [HttpGet(template: "{id}", Name = "QuerySecretByIdAsync")]
    [Produces(MediaTypeNames.Text.Plain, MediaTypeNames.Application.Json, MediaTypeNames.Multipart.Mixed)]

    [ProducesResponseType(type: typeof(SecretResponse), statusCode: StatusCodes.Status201Created)]
    public async Task<IActionResult> QuerySecretByIdAsync([FromRoute] Guid id)
    {
        var table = await _repository.QueryById(id);
        if (table is null)
        {
            return NotFound();
        }

        var (fileName, decryptedStream) = await _criptographyService.DecryptAsync(table);

        return HttpContext.Request.GetTypedHeaders().Accept switch
        {
            var accept when accept.Includes(MediaTypeNames.Text.Plain) => File(
                decryptedStream.ToArray(),
                MediaTypeNames.Text.Plain,
                fileName),
            var accept when accept.Includes(MediaTypeNames.Application.Json) => Ok(
                    SecretResponse.From(id: id, fileName: fileName, stream: decryptedStream)),
            _ => BadRequest(new ProblemDetails()
            {
                Type = "Bad Request",
                Title = "Invalid Request data",
                Detail = string.Format(
                    NonSupportedMidia,
                    string.Join(',', HttpContext.Request.GetTypedHeaders().Accept.Select(a => a.MediaType.Value))),
            }),
        };
    }

    [HttpGet]
    [ProducesResponseType(type: typeof(SecretLookupResponse[]), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _repository.AllAsync();
        return Ok(result.Select(SecretLookupResponse.From));
    }

    [HttpPost]
    [ProducesResponseType(type: typeof(SecretResponse[]), statusCode: StatusCodes.Status201Created)]
    public async Task<IActionResult> SaveSecretAsync([FromBody] SecretSaveModel secret)
    {
        var (content, iv) = _criptographyService.Encrypt(secret);

        var id = await _repository.SaveAsync(content, iv, secret.FileName);

        await content.DisposeAsync();

        return CreatedAtRoute(
            routeName: nameof(QuerySecretByIdAsync),
            routeValues: new { id },
            value: null);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var success = await _repository.Delete(id);

        if (success)
        {
            return NoContent();
        }

        return NotFound();
    }
}
