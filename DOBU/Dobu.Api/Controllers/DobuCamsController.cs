using Dobu.Domain.Entities;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Dobu.Api.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/dobucams")]
public class DobuCamsController(IDobuUnitOfWork context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DobuCam>>> GetAll([FromQuery] QueryParameters query)
    {
        return Ok(await context.DobuCams.ListAsync(query, includes: [x => x.Pet]));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DobuCam>> GetById(Guid id)
    {
        var camera = await context.DobuCams.FirstOrDefaultAsync(x => x.Id == id, tracking: false);
        return camera is null ? NotFound() : Ok(camera);
    }

    [HttpGet("pet/{petId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DobuCam>>> GetByPet(Guid petId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.DobuCams.ListAsync(query, predicate: x => x.PetId == petId));
    }

    [HttpGet("status/{statusCamera}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DobuCam>>> GetByStatus(string statusCamera, [FromQuery] QueryParameters query)
    {
        return Ok(await context.DobuCams.ListAsync(query, predicate: x => x.StatusCamera == statusCamera));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DobuCam>> Create(DobuCamRequest request)
    {
        if (await context.Pets.CountAsync(x => x.Id == request.PetId) == 0)
            return BadRequest("Pet informado nao existe.");

        try
        {
            var camera = new DobuCam(request.Localizacao, request.StatusCamera, request.DataUltimaMovimentacao, request.PetId);
            context.DobuCams.Add(camera);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = camera.Id }, camera);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DobuCam>> Update(Guid id, DobuCamRequest request)
    {
        var camera = await context.DobuCams.FirstOrDefaultAsync(x => x.Id == id);
        if (camera is null)
            return NotFound();

        if (await context.Pets.CountAsync(x => x.Id == request.PetId) == 0)
            return BadRequest("Pet informado nao existe.");

        try
        {
            camera.Atualizar(request.Localizacao, request.StatusCamera, request.DataUltimaMovimentacao, request.PetId);
            await context.SaveChangesAsync();
            return Ok(camera);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var camera = await context.DobuCams.FirstOrDefaultAsync(x => x.Id == id);
        if (camera is null)
            return NotFound();

        context.DobuCams.Remove(camera);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record DobuCamRequest(string Localizacao, string StatusCamera, DateTime? DataUltimaMovimentacao, Guid PetId);
