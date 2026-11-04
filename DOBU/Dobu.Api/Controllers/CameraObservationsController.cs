using System.Security.Claims;
using Dobu.Application.Queries;
using Dobu.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dobu.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dobucams/{cameraId:guid}/eventos")]
public sealed class CameraObservationsController(CameraObservationService observations) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid cameraId, [FromQuery] QueryParameters query, [FromQuery] string? tipo, CancellationToken cancellationToken) =>
        Ok(await observations.ListAsync(cameraId, UserId(), query, tipo, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid cameraId, Guid id, CancellationToken cancellationToken) =>
        Ok(await observations.GetAsync(cameraId, UserId(), id, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(Guid cameraId, CameraObservationRequest request, CancellationToken cancellationToken)
    {
        var record = await observations.CreateAsync(cameraId, UserId(), request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { cameraId, id = record.Id }, record);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid cameraId, Guid id, CancellationToken cancellationToken)
    {
        await observations.DeleteAsync(cameraId, UserId(), id, cancellationToken);
        return NoContent();
    }

    private Guid UserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : throw new UnauthorizedAccessException();
}
