using Dobu.Domain.Entities;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Dobu.Api.Controllers;

[ApiController]
[Route("api/especies")]
public class EspeciesController(IDobuUnitOfWork context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Especie>>> GetAll([FromQuery] QueryParameters query)
    {
        return Ok(await context.Especies.ListAsync(query, includes: [x => x.Racas]));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Especie>> GetById(Guid id)
    {
        var especie = await context.Especies.FirstOrDefaultAsync(x => x.Id == id, tracking: false, includes: [x => x.Racas]);
        return especie is null ? NotFound() : Ok(especie);
    }

    [HttpGet("nome/{nome}")]
    public async Task<ActionResult<IEnumerable<Especie>>> GetByNome(string nome, [FromQuery] QueryParameters query)
    {
        return Ok(await context.Especies.ListAsync(query, predicate: x => x.Nome.Contains(nome)));
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
