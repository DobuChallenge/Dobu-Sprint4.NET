using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/analises-ia")]
public class AnalisesIaController(DobuDbContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AnaliseIa>>> GetAll()
    {
        return Ok(await context.AnalisesIa.AsNoTracking().Include(x => x.Prontuario).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnaliseIa>> GetById(Guid id)
    {
        var analise = await context.AnalisesIa.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return analise is null ? NotFound() : Ok(analise);
    }

    [HttpGet("prontuario/{prontuarioId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AnaliseIa>> GetByProntuario(Guid prontuarioId)
    {
        var analise = await context.AnalisesIa.AsNoTracking().FirstOrDefaultAsync(x => x.ProntuarioId == prontuarioId);
        return analise is null ? NotFound() : Ok(analise);
    }

    [HttpGet("risco/{risco:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AnaliseIa>>> GetByRisco(int risco)
    {
        return Ok(await context.AnalisesIa.AsNoTracking().Where(x => x.Risco >= risco).ToListAsync());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AnaliseIa>> Create(AnaliseIaRequest request)
    {
        if (await context.Prontuarios.CountAsync(x => x.Id == request.ProntuarioId) == 0)
            return BadRequest("Prontuario informado nao existe.");

        if (await context.AnalisesIa.CountAsync(x => x.ProntuarioId == request.ProntuarioId) > 0)
            return BadRequest("Prontuario ja possui analise IA cadastrada.");

        try
        {
            var analise = new AnaliseIa(request.Descricao, request.Risco, request.DataAnalise, request.ProntuarioId);
            context.AnalisesIa.Add(analise);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = analise.Id }, analise);
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
    public async Task<ActionResult<AnaliseIa>> Update(Guid id, AnaliseIaRequest request)
    {
        var analise = await context.AnalisesIa.FirstOrDefaultAsync(x => x.Id == id);
        if (analise is null)
            return NotFound();

        if (await context.Prontuarios.CountAsync(x => x.Id == request.ProntuarioId) == 0)
            return BadRequest("Prontuario informado nao existe.");

        try
        {
            analise.Atualizar(request.Descricao, request.Risco, request.DataAnalise, request.ProntuarioId);
            await context.SaveChangesAsync();
            return Ok(analise);
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
        var analise = await context.AnalisesIa.FirstOrDefaultAsync(x => x.Id == id);
        if (analise is null)
            return NotFound();

        context.AnalisesIa.Remove(analise);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record AnaliseIaRequest(string Descricao, int Risco, DateTime DataAnalise, Guid ProntuarioId);
