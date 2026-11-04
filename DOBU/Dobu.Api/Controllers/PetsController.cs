using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[ApiController]
[Route("api/pets")]
[Authorize]
public class PetsController(DobuDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pet>>> GetAll()
    {
        return Ok(await context.Pets.AsNoTracking()
            .Include(x => x.Raca)
            .Include(x => x.Responsavel)
            .ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Pet>> GetById(Guid id)
    {
        var pet = await context.Pets.AsNoTracking()
            .Include(x => x.Raca)
            .Include(x => x.Responsavel)
            .FirstOrDefaultAsync(x => x.Id == id);

        return pet is null ? NotFound() : Ok(pet);
    }

    [HttpGet("responsavel/{responsavelId:guid}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByResponsavel(Guid responsavelId)
    {
        return Ok(await context.Pets.AsNoTracking().Where(x => x.ResponsavelId == responsavelId).ToListAsync());
    }

    [HttpGet("raca/{racaId:guid}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByRaca(Guid racaId)
    {
        return Ok(await context.Pets.AsNoTracking().Where(x => x.RacaId == racaId).ToListAsync());
    }

    [HttpGet("nome/{nome}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByNome(string nome)
    {
        return Ok(await context.Pets.AsNoTracking().Where(x => x.Nome.Contains(nome)).ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Pet>> Create(PetRequest request)
    {
        if (await context.Racas.CountAsync(x => x.Id == request.RacaId) == 0)
            return BadRequest("Raca informada nao existe.");

        var responsavel = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == request.ResponsavelId);
        if (responsavel is null || responsavel.TipoUsuario != "RESPONSAVEL")
            return BadRequest("Responsavel informado nao existe ou nao possui perfil RESPONSAVEL.");

        try
        {
            var pet = new Pet(request.Nome, request.Idade, request.RacaId, request.ResponsavelId);
            context.Pets.Add(pet);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Pet>> Update(Guid id, PetRequest request)
    {
        var pet = await context.Pets.FirstOrDefaultAsync(x => x.Id == id);
        if (pet is null)
            return NotFound();

        if (await context.Racas.CountAsync(x => x.Id == request.RacaId) == 0)
            return BadRequest("Raca informada nao existe.");

        var responsavel = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == request.ResponsavelId);
        if (responsavel is null || responsavel.TipoUsuario != "RESPONSAVEL")
            return BadRequest("Responsavel informado nao existe ou nao possui perfil RESPONSAVEL.");

        try
        {
            pet.Atualizar(request.Nome, request.Idade, request.RacaId, request.ResponsavelId);
            await context.SaveChangesAsync();
            return Ok(pet);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var pet = await context.Pets.FirstOrDefaultAsync(x => x.Id == id);
        if (pet is null)
            return NotFound();

        context.Pets.Remove(pet);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record PetRequest(string Nome, int Idade, Guid RacaId, Guid ResponsavelId);
