using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[ApiController]
[Route("api/racas")]
public class RacasController(DobuDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Raca>>> GetAll()
    {
        return Ok(await context.Racas.AsNoTracking().Include(x => x.Especie).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Raca>> GetById(Guid id)
    {
        var raca = await context.Racas.AsNoTracking().Include(x => x.Especie).FirstOrDefaultAsync(x => x.Id == id);
        return raca is null ? NotFound() : Ok(raca);
    }

    [HttpGet("especie/{especieId:guid}")]
    public async Task<ActionResult<IEnumerable<Raca>>> GetByEspecie(Guid especieId)
    {
        return Ok(await context.Racas.AsNoTracking().Where(x => x.EspecieId == especieId).ToListAsync());
    }

    [HttpGet("porte/{porte}")]
    public async Task<ActionResult<IEnumerable<Raca>>> GetByPorte(string porte)
    {
        return Ok(await context.Racas.AsNoTracking().Where(x => x.Porte == porte).ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Raca>> Create(RacaRequest request)
    {
        if (await context.Especies.CountAsync(x => x.Id == request.EspecieId) == 0)
            return BadRequest("Especie informada nao existe.");

        try
        {
            var raca = new Raca(request.Nome, request.Porte, request.ExpectativaVida, request.Descricao, request.Cuidados, request.EspecieId);
            context.Racas.Add(raca);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = raca.Id }, raca);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Raca>> Update(Guid id, RacaRequest request)
    {
        var raca = await context.Racas.FirstOrDefaultAsync(x => x.Id == id);
        if (raca is null)
            return NotFound();

        if (await context.Especies.CountAsync(x => x.Id == request.EspecieId) == 0)
            return BadRequest("Especie informada nao existe.");

        try
        {
            raca.Atualizar(request.Nome, request.Porte, request.ExpectativaVida, request.Descricao, request.Cuidados, request.EspecieId);
            await context.SaveChangesAsync();
            return Ok(raca);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var raca = await context.Racas.FirstOrDefaultAsync(x => x.Id == id);
        if (raca is null)
            return NotFound();

        context.Racas.Remove(raca);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record RacaRequest(string Nome, string Porte, int ExpectativaVida, string Descricao, string Cuidados, Guid EspecieId);
