using Dobu.Domain.Entities;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Dobu.Api.Controllers;

[ApiController]
[Route("api/consultas")]
public class ConsultasController(IDobuUnitOfWork context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Consulta>>> GetAll([FromQuery] QueryParameters query)
    {
        return Ok(await context.Consultas.ListAsync(query, includes: [x => x.Pet, x => x.Veterinario]));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Consulta>> GetById(Guid id)
    {
        var consulta = await context.Consultas.FirstOrDefaultAsync(x => x.Id == id, tracking: false, includes: [x => x.Pet, x => x.Veterinario]);

        return consulta is null ? NotFound() : Ok(consulta);
    }

    [HttpGet("pet/{petId:guid}")]
    public async Task<ActionResult<IEnumerable<Consulta>>> GetByPet(Guid petId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Consultas.ListAsync(query, predicate: x => x.PetId == petId));
    }

    [HttpGet("veterinario/{veterinarioId:guid}")]
    public async Task<ActionResult<IEnumerable<Consulta>>> GetByVeterinario(Guid veterinarioId, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Consultas.ListAsync(query, predicate: x => x.VeterinarioId == veterinarioId));
    }

    [HttpGet("periodo")]
    public async Task<ActionResult<IEnumerable<Consulta>>> GetByPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fim, [FromQuery] QueryParameters query)
    {
        if (inicio > fim)
            return BadRequest("A data inicial deve ser menor ou igual a data final.");

        return Ok(await context.Consultas.ListAsync(query, predicate: x => x.DataConsulta >= inicio && x.DataConsulta <= fim));
    }

    [HttpPost]
    public async Task<ActionResult<Consulta>> Create(ConsultaRequest request)
    {
        if (await context.Pets.CountAsync(x => x.Id == request.PetId) == 0)
            return BadRequest("Pet informado nao existe.");

        var veterinario = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == request.VeterinarioId);
        if (veterinario is null || veterinario.TipoUsuario != "VETERINARIO")
            return BadRequest("Veterinario informado nao existe ou nao possui perfil VETERINARIO.");

        try
        {
            var consulta = new Consulta(request.DataConsulta, request.Descricao, request.Valor, request.PetId, request.VeterinarioId);
            context.Consultas.Add(consulta);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = consulta.Id }, consulta);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Consulta>> Update(Guid id, ConsultaRequest request)
    {
        var consulta = await context.Consultas.FirstOrDefaultAsync(x => x.Id == id);
        if (consulta is null)
            return NotFound();

        if (await context.Pets.CountAsync(x => x.Id == request.PetId) == 0)
            return BadRequest("Pet informado nao existe.");

        var veterinario = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == request.VeterinarioId);
        if (veterinario is null || veterinario.TipoUsuario != "VETERINARIO")
            return BadRequest("Veterinario informado nao existe ou nao possui perfil VETERINARIO.");

        try
        {
            consulta.Atualizar(request.DataConsulta, request.Descricao, request.Valor, request.PetId, request.VeterinarioId);
            await context.SaveChangesAsync();
            return Ok(consulta);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var consulta = await context.Consultas.FirstOrDefaultAsync(x => x.Id == id);
        if (consulta is null)
            return NotFound();

        context.Consultas.Remove(consulta);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record ConsultaRequest(DateTime DataConsulta, string Descricao, decimal Valor, Guid PetId, Guid VeterinarioId);
