using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/prontuarios")]
public class ProntuariosController(DobuDbContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Prontuario>>> GetAll()
    {
        return Ok(await context.Prontuarios.AsNoTracking().Include(x => x.Consulta).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Prontuario>> GetById(Guid id)
    {
        var prontuario = await context.Prontuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return prontuario is null ? NotFound() : Ok(prontuario);
    }

    [HttpGet("consulta/{consultaId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Prontuario>> GetByConsulta(Guid consultaId)
    {
        var prontuario = await context.Prontuarios.AsNoTracking().FirstOrDefaultAsync(x => x.ConsultaId == consultaId);
        return prontuario is null ? NotFound() : Ok(prontuario);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Prontuario>> Create(ProntuarioRequest request)
    {
        if (await context.Consultas.CountAsync(x => x.Id == request.ConsultaId) == 0)
            return BadRequest("Consulta informada nao existe.");

        if (await context.Prontuarios.CountAsync(x => x.ConsultaId == request.ConsultaId) > 0)
            return BadRequest("Consulta ja possui prontuario cadastrado.");

        try
        {
            var prontuario = new Prontuario(request.Diagnostico, request.Observacoes, request.ConsultaId);
            context.Prontuarios.Add(prontuario);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = prontuario.Id }, prontuario);
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
    public async Task<ActionResult<Prontuario>> Update(Guid id, ProntuarioRequest request)
    {
        var prontuario = await context.Prontuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (prontuario is null)
            return NotFound();

        if (await context.Consultas.CountAsync(x => x.Id == request.ConsultaId) == 0)
            return BadRequest("Consulta informada nao existe.");

        try
        {
            prontuario.Atualizar(request.Diagnostico, request.Observacoes, request.ConsultaId);
            await context.SaveChangesAsync();
            return Ok(prontuario);
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
        var prontuario = await context.Prontuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (prontuario is null)
            return NotFound();

        context.Prontuarios.Remove(prontuario);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record ProntuarioRequest(string Diagnostico, string Observacoes, Guid ConsultaId);
