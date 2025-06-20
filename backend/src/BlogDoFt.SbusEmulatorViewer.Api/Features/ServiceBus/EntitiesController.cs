using BlogDoFt.SbusEmulatorViewer.Api.Features.ServiceBus.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.SbusEmulatorViewer.Api.Features.ServiceBus;

[ApiController]
[Route("api/[controller]")]
public class EntitiesController : ControllerBase
{
    private readonly EntitiesParser _entities;

    public EntitiesController(EntitiesParser entities)
    {
        _entities = entities;
    }

    [HttpGet]
    public async Task<IActionResult> ListEntitiesAsync(CancellationToken cancellation = default)
    {
        var topics = await _entities.GetTopicsAsync(cancellation);
        if (topics.Any())
        {
            return Ok(topics);
        }

        return NotFound();
    }
}
