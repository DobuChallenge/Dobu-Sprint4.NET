using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/informacoes-cuidado")]
public class InformacoesCuidadoController(DobuDbContext context) : ControllerBase
{
    [HttpGet("pet/{petId:guid}")]
    public async Task<ActionResult<IEnumerable<InformacaoCuidado>>> GetByPet(Guid petId, CancellationToken cancellationToken) =>
        Ok(await context.InformacoesCuidado.AsNoTracking().Where(x => x.PetId == petId).ToListAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<InformacaoCuidado>> Create(InformacaoCuidadoRequest request, CancellationToken cancellationToken)
    {
        if (!await context.Pets.AnyAsync(x => x.Id == request.PetId, cancellationToken)) return BadRequest("Pet informado não existe.");
        try
        {
            var information = new InformacaoCuidado(request.Titulo, request.Descricao, request.PetId);
            context.InformacoesCuidado.Add(information);
            await context.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(GetByPet), new { petId = information.PetId }, information);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var information = await context.InformacoesCuidado.FindAsync([id], cancellationToken);
        if (information is null) return NotFound();
        context.InformacoesCuidado.Remove(information);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public record InformacaoCuidadoRequest(string Titulo, string Descricao, Guid PetId);
