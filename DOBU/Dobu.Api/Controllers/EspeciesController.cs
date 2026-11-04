using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[ApiController]
[Route("api/especies")]
public class EspeciesController(DobuDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Especie>>> GetAll()
    {
        return Ok(await context.Especies.AsNoTracking().Include(x => x.Racas).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Especie>> GetById(Guid id)
    {
        var especie = await context.Especies.AsNoTracking().Include(x => x.Racas).FirstOrDefaultAsync(x => x.Id == id);
        return especie is null ? NotFound() : Ok(especie);
    }

    [HttpGet("nome/{nome}")]
    public async Task<ActionResult<IEnumerable<Especie>>> GetByNome(string nome)
    {
        return Ok(await context.Especies.AsNoTracking().Where(x => x.Nome.Contains(nome)).ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Especie>> Create(EspecieRequest request)
    {
        try
        {
            var especie = new Especie(request.Nome, request.Descricao);
            context.Especies.Add(especie);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = especie.Id }, especie);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Especie>> Update(Guid id, EspecieRequest request)
    {
        var especie = await context.Especies.FirstOrDefaultAsync(x => x.Id == id);
        if (especie is null)
            return NotFound();

        try
        {
            especie.Atualizar(request.Nome, request.Descricao);
            await context.SaveChangesAsync();
            return Ok(especie);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var especie = await context.Especies.FirstOrDefaultAsync(x => x.Id == id);
        if (especie is null)
            return NotFound();

        context.Especies.Remove(especie);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record EspecieRequest(string Nome, string Descricao);
