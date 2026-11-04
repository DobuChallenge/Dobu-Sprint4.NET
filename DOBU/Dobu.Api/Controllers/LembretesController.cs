using Dobu.Domain.Entities;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Dobu.Api.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/lembretes")]
public class LembretesController(IDobuUnitOfWork context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Lembrete>>> GetAll([FromQuery] QueryParameters query)
    {
        return Ok(await context.Lembretes.ListAsync(query, includes: [x => x.Pet]));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Lembrete>> GetById(Guid id)
    {
        var lembrete = await context.Lembretes.FirstOrDefaultAsync(x => x.Id == id, tracking: false);
        return lembrete is null ? NotFound() : Ok(lembrete);
    }

    [HttpGet("pet/{petId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Lembrete>>> GetByPet(Guid petId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Lembretes.ListAsync(query, predicate: x => x.PetId == petId));
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Lembrete>>> GetByStatus(string status, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Lembretes.ListAsync(query, predicate: x => x.Status == status));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Lembrete>> Create(LembreteRequest request)
    {
        if (await context.Pets.CountAsync(x => x.Id == request.PetId) == 0)
            return BadRequest("Pet informado nao existe.");

        try
        {
            var lembrete = new Lembrete(request.Descricao, request.DataLembrete, request.Status, request.PetId);
            context.Lembretes.Add(lembrete);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = lembrete.Id }, lembrete);
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
    public async Task<ActionResult<Lembrete>> Update(Guid id, LembreteRequest request)
    {
        var lembrete = await context.Lembretes.FirstOrDefaultAsync(x => x.Id == id);
        if (lembrete is null)
            return NotFound();

        if (await context.Pets.CountAsync(x => x.Id == request.PetId) == 0)
            return BadRequest("Pet informado nao existe.");

        try
        {
            lembrete.Atualizar(request.Descricao, request.DataLembrete, request.Status, request.PetId);
            await context.SaveChangesAsync();
            return Ok(lembrete);
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
        var lembrete = await context.Lembretes.FirstOrDefaultAsync(x => x.Id == id);
        if (lembrete is null)
            return NotFound();

        context.Lembretes.Remove(lembrete);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record LembreteRequest(string Descricao, DateTime DataLembrete, string Status, Guid PetId);
