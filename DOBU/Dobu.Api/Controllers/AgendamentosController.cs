using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/agendamentos")]
[Authorize]
public class AgendamentosController(DobuDbContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetAll()
    {
        return Ok(await context.Agendamentos.AsNoTracking().Include(x => x.Pet).Include(x => x.Veterinario).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Agendamento>> GetById(Guid id)
    {
        var agendamento = await context.Agendamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return agendamento is null ? NotFound() : Ok(agendamento);
    }

    [HttpGet("pet/{petId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetByPet(Guid petId)
    {
        return Ok(await context.Agendamentos.AsNoTracking().Where(x => x.PetId == petId).ToListAsync());
    }

    [HttpGet("veterinario/{veterinarioId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetByVeterinario(Guid veterinarioId)
    {
        return Ok(await context.Agendamentos.AsNoTracking().Where(x => x.VeterinarioId == veterinarioId).ToListAsync());
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetByStatus(string status)
    {
        return Ok(await context.Agendamentos.AsNoTracking().Where(x => x.Status == status).ToListAsync());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Agendamento>> Create(AgendamentoRequest request)
    {
        if (await context.Pets.CountAsync(x => x.Id == request.PetId) == 0)
            return BadRequest("Pet informado nao existe.");

        var veterinario = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == request.VeterinarioId);
        if (veterinario is null || veterinario.TipoUsuario != "VETERINARIO")
            return BadRequest("Veterinario informado nao existe ou nao possui perfil VETERINARIO.");

        try
        {
            var agendamento = new Agendamento(request.DataAgendamento, request.Status, request.PetId, request.VeterinarioId);
            context.Agendamentos.Add(agendamento);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = agendamento.Id }, agendamento);
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
    public async Task<ActionResult<Agendamento>> Update(Guid id, AgendamentoRequest request)
    {
        var agendamento = await context.Agendamentos.FirstOrDefaultAsync(x => x.Id == id);
        if (agendamento is null)
            return NotFound();

        if (await context.Pets.CountAsync(x => x.Id == request.PetId) == 0)
            return BadRequest("Pet informado nao existe.");

        var veterinario = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == request.VeterinarioId);
        if (veterinario is null || veterinario.TipoUsuario != "VETERINARIO")
            return BadRequest("Veterinario informado nao existe ou nao possui perfil VETERINARIO.");

        try
        {
            agendamento.Atualizar(request.DataAgendamento, request.Status, request.PetId, request.VeterinarioId);
            await context.SaveChangesAsync();
            return Ok(agendamento);
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
        var agendamento = await context.Agendamentos.FirstOrDefaultAsync(x => x.Id == id);
        if (agendamento is null)
            return NotFound();

        context.Agendamentos.Remove(agendamento);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record AgendamentoRequest(DateTime DataAgendamento, string Status, Guid PetId, Guid VeterinarioId);
