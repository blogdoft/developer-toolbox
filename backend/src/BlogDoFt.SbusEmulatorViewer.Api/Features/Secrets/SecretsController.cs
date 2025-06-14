using BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Impl;
using BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;

namespace BlogDoFt.SbusEmulatorViewer.Api.Features.Secrets;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class SecretsController : ControllerBase
{
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
    [ProducesResponseType(type: typeof(SecretResponse), statusCode: StatusCodes.Status201Created)]
    public async Task<IActionResult> QuerySecretByIdAsync([FromRoute] Guid id)
    {
        var table = await _repository.QueryById(id);
        if (table is null)
        {
            return NotFound();
        }

        var (fileName, decryptedStream) = await _criptographyService.DecryptAsync(table);

        return Ok(new SecretResponse()
        {
            Id = id,
            FileName = fileName,
            Content = Encoding.UTF8.GetString(decryptedStream.ToArray()),
        });
    }

    [HttpPost]
    public async Task<IActionResult> SaveSecretAsync([FromBody] SecretSaveModel secret)
    {
        var (text, iv) = _criptographyService.Encrypt(secret);

        var id = await _repository.SaveAsync(text, iv, secret.FileName);

        await text.DisposeAsync();

        return CreatedAtRoute(
            routeName: nameof(QuerySecretByIdAsync),
            routeValues: new { id },
            value: null);
    }
}
