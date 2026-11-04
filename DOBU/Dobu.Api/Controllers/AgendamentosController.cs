using Dobu.Application.Services;
using Dobu.Domain.Entities;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Dobu.Api.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/agendamentos")]
[Authorize]
public class AgendamentosController(IDobuUnitOfWork context, AgendamentoService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetAll([FromQuery] QueryParameters query)
    {
        return Ok(await context.Agendamentos.ListAsync(query, includes: [x => x.Pet, x => x.Veterinario]));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Agendamento>> GetById(Guid id)
    {
        var agendamento = await context.Agendamentos.FirstOrDefaultAsync(x => x.Id == id, tracking: false);
        return agendamento is null ? NotFound() : Ok(agendamento);
    }

    [HttpGet("pet/{petId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetByPet(Guid petId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Agendamentos.ListAsync(query, predicate: x => x.PetId == petId));
    }

    [HttpGet("veterinario/{veterinarioId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetByVeterinario(Guid veterinarioId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Agendamentos.ListAsync(query, predicate: x => x.VeterinarioId == veterinarioId));
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Agendamento>>> GetByStatus(string status, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Agendamentos.ListAsync(query, predicate: x => x.Status == status));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Agendamento>> Create(AgendamentoRequest request)
    {
        var appointment = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Agendamento>> Update(Guid id, AgendamentoRequest request)
    {
        return Ok(await service.UpdateAsync(id, request));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
